using System;

namespace DomainInfoCore.DataObject
{
    public class TaskResultItem
    {
        public Int64 ID { get; set; }
        public string IP { get; set; }
        public TaskType TaskType { get; set; }
        public DateTime TS { get; set; }
        public DateTime QTS { get; set; }
        public string Data { get; set; }
        public TaskState State { get; set; }
        public int ReportCount { get; set; }
    }
}
