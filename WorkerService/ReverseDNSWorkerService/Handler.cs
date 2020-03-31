using System;
using System.Net;
using Microsoft.Extensions.Logging;
using WorkerCore;
using WorkerCore.DomainInfo;

namespace ReverseDNSWorkerService
{
    public class Handler : basehandler
    {
        private readonly ILogger<Worker> _logger;
        public Handler(string url, string taskname, ILogger<Worker> logger) : base(url, taskname) { _logger = logger; }
        public override void OnException(Exception ex) => _logger.LogError(ex, "Error");
        public override WorkerReportItem Process(WorkerQueueItem wqi)
        {
            var result = new WorkerReportItem() { Id = wqi.Id, Ip = wqi.Ip, Rprtcnt = wqi.Rpcnt, Qts = wqi.Qts };
            try
            {
                System.Net.IPAddress address;
                if (System.Net.IPAddress.TryParse(wqi.Ip, out address))
                {
                    //an ip
                    result.Message = Dns.GetHostEntry(wqi.Ip).HostName;
                    result.State = "Complete";
                }
                else
                {
                    //domain
                    result.Message = Dns.GetHostAddresses(wqi.Ip)[0].ToString();
                    result.State = "Complete";
                }
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
                result.State = "Error";
            }
            result.Ts = DateTime.Now;
            return result;
        }
    }
}