using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using WorkerCore;
using WorkerCore.DomainInfo;

namespace GeoIPWorkerService
{
    public class Handler : basehandler
    {
        private readonly ILogger<Worker> _logger;
        public Handler(string url, string taskname, ILogger<Worker> logger) : base(url, taskname) { _logger = logger; }
        public override void OnException(Exception ex) => _logger.LogError(ex, "Error");
        public override WorkerReportItem Process(WorkerQueueItem wqi)
        {
            var result = new WorkerReportItem() { Id = wqi.Id, Ip = wqi.Ip, Rprtcnt = wqi.Rpcnt, Qts = wqi.Qts };
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format(
                "http://api.ipstack.com/{0}?access_key=7ac0e3b02aacd365123419fa5fcf8078&format=1", wqi.Ip));
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
                    //sb.Append(responseFromServer);
                    sb.Append($"continent_code:{myJObject.SelectToken("continent_code").Value<string>()},");
                    sb.Append($"continent_name:{myJObject.SelectToken("continent_name").Value<string>()},");
                    sb.Append($"country_code:{myJObject.SelectToken("country_code").Value<string>()},");
                    sb.Append($"country_name:{myJObject.SelectToken("country_name").Value<string>()},");
                    sb.Append($"region_code:{myJObject.SelectToken("region_code").Value<string>()},");
                    sb.Append($"region_name:{myJObject.SelectToken("region_name").Value<string>()},");
                    sb.Append($"city:{myJObject.SelectToken("city").Value<string>()},");
                    sb.Append($"zip:{myJObject.SelectToken("zip").Value<string>()},");
                    sb.Append($"latitude:{myJObject.SelectToken("zip").Value<double>()},");
                    sb.Append($"longitude:{myJObject.SelectToken("zip").Value<double>()}");
                }
                result.Data = sb.ToString();
                result.State = "Complete";
            }
            catch (Exception ex)
            {
                result.Data = $"{ex.Message}:{ex.StackTrace}";
                result.State = "Error";
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();
            }
            result.Ts = DateTime.Now;
            return result;
        }
    }
}
