﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PingWorkerService
{
    public class WorkerReportItem
    {
        public Int64 id { get; set; }
        public string ip { get; set; }
        public string message { get; set; }
        public string state { get; set; }
        public DateTime ts { get; set; }
        public int rprtcnt { get; set; }
        public DateTime qts { get; set; }
    }
}
