using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using WorkerCore.DomainInfo;
using System.Linq;

namespace WorkerCore
{
    public abstract class basehandler
    {
        protected readonly string taskname;
        protected readonly string url;
        public List<WorkerQueueItem> queue = new List<WorkerQueueItem>();
        public List<WorkerReportItem> report = new List<WorkerReportItem>();

        public basehandler(string url, string taskname)
        {
            this.url = url;
            this.taskname = taskname;
        }

        public abstract void OnException(Exception ex);

        public async Task Request()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    DomainInfo.DomainInfoClient swclient = new DomainInfo.DomainInfoClient(this.url, client);
                    var result = (await swclient.WorkerAllAsync(taskname)).ToList();
                    queue.AddRange(result);
                }
                catch (Exception ex)
                {
                    OnException(ex);
                }
            }
        }

        public abstract WorkerReportItem Process(WorkerQueueItem wqi);
        public async Task ReportToAPI(List<WorkerReportItem> ritems)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    DomainInfo.DomainInfoClient swclient = new DomainInfo.DomainInfoClient(this.url, client);
                    await swclient.WorkerAsync(taskname, ritems);
                }
                catch (Exception ex)
                {
                    OnException(ex);
                }
            }
        }
    }
}
