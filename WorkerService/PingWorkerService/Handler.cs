using System;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Extensions.Logging;
using WorkerCore;

namespace PingWorkerService
{
    public class Handler : basehandler
    {
        private readonly ILogger<Worker> _logger;
        public Handler(string url, string taskname, ILogger<Worker> logger) : base(url, taskname) { _logger = logger; }
        public override void OnException(Exception ex) => _logger.LogError(ex, "Error");
        public override WorkerCore.WorkerReportItem Process(WorkerCore.WorkerQueueItem wqi)
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
    }
}
