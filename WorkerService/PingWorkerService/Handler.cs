using System;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Extensions.Logging;
using WorkerCore;
using WorkerCore.DomainInfo;

namespace PingWorkerService
{
    public class Handler : basehandler
    {
        private readonly ILogger<Worker> _logger;
        public Handler(string url, string taskname, ILogger<Worker> logger) : base(url, taskname) { _logger = logger; }
        public override void OnException(Exception ex) => _logger.LogError(ex, "Error");
        public override WorkerReportItem Process(WorkerQueueItem wqi)
        {
            var result = new WorkerReportItem() { Id = wqi.Id, Ip = wqi.Ip, Rprtcnt = wqi.Rpcnt, Qts = wqi.Qts };
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
                PingReply reply = pingSender.Send(wqi.Ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    sb.Append($"Address:{reply.Address},");
                    sb.Append($"RoundTrip:{reply.RoundtripTime},");
                    sb.Append($"TimeTolive:{reply.Options.Ttl},");
                    sb.Append($"DontFragment:{reply.Options.DontFragment},");
                    sb.Append($"BufferSize:{reply.Buffer.Length}");
                }
                else
                    sb.Append(reply.Status);
                result.State = "Complete";
            }
            catch (Exception ex)
            {
                result.State = "Complete";
                sb.Append($"Ping request could not find host: {ex.Message}");
            }
            result.Data = sb.ToString();
            result.Ts = DateTime.Now;
            return result;
        }
    }
}
