using System;
using System.Net;
using Microsoft.Extensions.Logging;
using WorkerCore;

namespace ReverseDNSWorkerService
{
    public class Handler : basehandler
    {
        private readonly ILogger<Worker> _logger;
        public Handler(string url, string taskname, ILogger<Worker> logger) : base(url, taskname) { _logger = logger; }
        public override void OnException(Exception ex) => _logger.LogError(ex, "Error");
        public override WorkerCore.WorkerReportItem Process(WorkerCore.WorkerQueueItem wqi)
        {
            var result = new WorkerReportItem() { id = wqi.id, ip = wqi.ip, rprtcnt = wqi.rpcnt, qts = wqi.qts };
            try
            {
                System.Net.IPAddress address;
                if (System.Net.IPAddress.TryParse(wqi.ip, out address))
                {
                    //an ip
                    result.message = Dns.GetHostEntry(wqi.ip).HostName;
                    result.state = "Complete";
                }
                else
                {
                    //domain
                    result.message = Dns.GetHostAddresses(wqi.ip)[0].ToString();
                    result.state = "Complete";
                }
            }
            catch (System.Exception ex)
            {
                result.message = ex.Message;
                result.state = "Error";
            }
            result.ts = DateTime.Now;
            return result;
        }
    }
}