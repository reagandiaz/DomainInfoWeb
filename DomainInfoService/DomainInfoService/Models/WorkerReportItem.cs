﻿using System;

namespace DomainInfoService.Models
{
    public class WorkerReportItem
    {
        public Int64 id { get; set; }
        public string ip { get; set; }
        public string task { get; set; }
        public string data { get; set; }
        public string state { get; set; }
        public DateTime ts { get; set; }
        public int rprtcnt { get; set; }
        public DateTime qts { get; set; }
    }
}