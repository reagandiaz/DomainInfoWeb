using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PingWorkerService
{
    public class Worker : BackgroundService
    {
        string taskname = "Ping";
        private readonly ILogger<Worker> _logger;
        private List<WorkerQueueItem> queue = new List<WorkerQueueItem>();
        private List<WorkerReportItem> report = new List<WorkerReportItem>();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        volatile bool requestbusy = false;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!requestbusy)
                {
                    requestbusy = true;
                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        Request();
                        requestbusy = false;
                    });
                }

                List<WorkerQueueItem> toprocess = new List<WorkerQueueItem>();

                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        toprocess.AddRange(queue.ToList());
                        queue.Clear();
                    }
                }

                toprocess.ForEach(s =>
                {
                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        WorkerReportItem repitem = Process(s);
                        lock (report)
                        {
                            report.Add(repitem);
                        }
                    });
                });

                var toupload = new List<WorkerReportItem>();

                lock (report)
                {
                    if (report.Count > 0)
                    {
                        toupload.AddRange(report.ToList());
                        report.Clear();
                    }
                }

                if (toupload.Count > 0)
                {
                    ReportToAPI(toupload);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
        void Request()
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format(
                "http://localhost:1108/Worker?task={0}", taskname));
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
                _logger.LogError(ex, "Error");
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();
            }
        }
        WorkerReportItem Process(WorkerQueueItem wqi)
        {
            var result = new WorkerReportItem() { id = wqi.id, ip = wqi.ip, rprtcnt = wqi.rpcnt, qts = wqi.qts };
            /*nuget System.NET.Ping*/

            StringBuilder sb = new StringBuilder();
            try
            {
                string data = "Hello! This is Reagan Diaz";
                var pingSender = new System.Net.NetworkInformation.Ping();
                PingOptions options = new PingOptions
                {
                    DontFragment = true
                };
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                const int timeout = 1024;
                PingReply reply = pingSender.Send(wqi.ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    sb.Append($"Address: {reply.Address} ");
                    sb.Append($"RoundTrip: {reply.RoundtripTime} ");
                    sb.Append($"TimeTolive: {reply.Options.Ttl} ");
                    sb.Append($"DontFragment: {reply.Options.DontFragment} ");
                    sb.Append($"BufferSize: {reply.Buffer.Length}");
                }
                else
                    sb.Append(reply.Status);
                result.state = "Complete";
            }
            catch (Exception ex)
            {
                result.state = "Error";
                sb.Append(ex.Message);
            }
            result.message = sb.ToString();
            result.ts = DateTime.Now;
            return result;
        }
        void ReportToAPI(List<WorkerReportItem> ritems)
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format(
                "http://localhost:1108/Worker?task={0}", taskname));
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
                _logger.LogError(ex, "Error");
            }
            finally
            {
                if (response != null) response.Close();
            }
        }
    }
}
