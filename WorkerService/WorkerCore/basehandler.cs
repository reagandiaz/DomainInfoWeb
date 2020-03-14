using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace WorkerCore
{
    public abstract class basehandler
    {
        readonly string taskname;
        readonly string url;
        public List<WorkerQueueItem> queue = new List<WorkerQueueItem>();
        public List<WorkerReportItem> report = new List<WorkerReportItem>();

        public basehandler(string url, string taskname)
        {
            this.url = url;
            this.taskname = taskname;
        }

        public abstract void OnException(Exception ex);
        public void Request()
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format("{0}?task={1}", url, taskname));
                request.Method = "PUT";
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                if (responseFromServer != "[]")
                {
                    var newqueue = JsonSerializer.Deserialize<WorkerQueueItem[]>(responseFromServer);
                    lock (queue)
                    {
                        queue.AddRange(newqueue.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();
            }
        }
        public abstract WorkerReportItem Process(WorkerQueueItem wqi);
        public void ReportToAPI(List<WorkerReportItem> ritems)
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format("{0}?task={1}", url, taskname));
                request.ContentType = "application/json";
                request.Method = "POST";
                var payload = JsonSerializer.Serialize<List<WorkerReportItem>>(ritems);
                request.ContentLength = payload.Length;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(payload);
                }
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            finally
            {
                if (response != null) response.Close();
            }
        }
    }
}
