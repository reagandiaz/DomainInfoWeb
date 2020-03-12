using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using IntegrationTools;
using Newtonsoft.Json.Linq;

namespace DomainInfoCore.Tasks
{
    public class GeoIP : TaskProcessTemplate
    {
        public GeoIP(Cache cache, TaskType tasktype) : base(cache, tasktype) { }

        public override TaskResultItem ExecuteProcess(TaskQueueItem queue)
        {
            var result = new TaskResultItem(queue);
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format(
                "http://api.ipstack.com/{0}?access_key=7ac0e3b02aacd365123419fa5fcf8078&format=1", queue.IP));
                // Get the response.
                response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.

                var myJObject = JObject.Parse(responseFromServer);

                StringBuilder sb = new StringBuilder();

                if (String.IsNullOrEmpty(myJObject.SelectToken("continent_name").Value<string>()))
                    sb.Append("Not Found");
                else
                {
                    sb.Append($"{myJObject.SelectToken("continent_name").Value<string>()}, ");
                    sb.Append($"{myJObject.SelectToken("country_name").Value<string>()}, ");
                    sb.Append($"{myJObject.SelectToken("region_name").Value<string>()}, ");
                    sb.Append($"{myJObject.SelectToken("city").Value<string>()} ");
                }
                result.Message = sb.ToString();
                result.State = TaskState.Complete;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = TaskState.Error;
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();

            }
            return result;
        }
    }

}
