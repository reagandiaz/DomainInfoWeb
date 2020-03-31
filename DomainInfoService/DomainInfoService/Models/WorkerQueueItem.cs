using DomainInfoCore.DataObject;
using System;

namespace DomainInfoService.Models
{
    public class WorkerQueueItem
    {
        public Int64 id { get; set; }

        public string ip { get; set; }

        public DateTime qts { get; set; }

        public int rpcnt { get; set; }

        public WorkerQueueItem(TaskQueueItem queue)
        {
            id = queue.ID;
            ip = queue.IP;
            rpcnt = queue.ReportCount;
            qts = queue.QueueTS;
        }
    }
}