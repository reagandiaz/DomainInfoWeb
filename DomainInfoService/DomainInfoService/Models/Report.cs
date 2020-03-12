using DomainInfoCore.DataObject;
using System;
using System.ComponentModel.DataAnnotations;

namespace DomainInfoService.Models
{
    public class Report
    {
        public Int64 id { get; set; }
        public string ip { get; set; }
        public ReportItem[] reports { get; set; }
        public string message { get; set; }

        public void Load(IPResult match)
        {
            ip = match.IP;
            reports = new Models.ReportItem[match.ReportCount];
            for (int i = 0; i < reports.Length; i++)
            {
                reports[i] = new Models.ReportItem()
                {
                    task = Enum.GetName(match.TaskReports[i].TaskType.GetType(), match.TaskReports[i].TaskType),
                    start = match.TaskReports[i].Start,
                    end = match.TaskReports[i].End,
                    message = match.TaskReports[i].Message,
                    state = Enum.GetName(match.TaskReports[i].State.GetType(), match.TaskReports[i].State),
                };
            }
        }
    }
}
