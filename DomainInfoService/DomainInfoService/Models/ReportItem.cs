using System;
using System.ComponentModel.DataAnnotations;

namespace DomainInfoService.Models
{
    public class ReportItem
    {
        public string task { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string data { get; set; }
        public string state { get; set; }
    }
}
