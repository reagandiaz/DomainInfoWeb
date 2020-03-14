using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerCore
{
    public class WorkerQueueItem
    {
        public Int64 id { get; set; }
        public string ip { get; set; }
        public DateTime qts { get; set; }
        public int rpcnt { get; set; }
    }
}
