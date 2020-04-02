using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DomainInfoCore.DataObject
{
    public class IPResult
    {
        const int expiration = 60; //1 minute delete 
        public Int64 ID => id;
        public string IP => ip;
        public List<TaskReport> TaskReports => tr;
        public int ReportCount => rpcnt;
        public bool Complete { get; set; }
        public bool Expired => exp;

        readonly Int64 id;
        readonly string ip;
        readonly List<TaskReport> tr;
        int rpcnt;
        bool exp;

        public DateTime CompleteTS { get; set; }

        public IPResult(Int64 id, string ip, int rpcnt)
        {
            this.id = id;
            this.ip = ip;
            this.rpcnt = rpcnt;
            tr = new List<TaskReport>();
            CompleteTS = DateTime.MaxValue;
        }
        public void AddResults(List<TaskResultItem> trs)
        {
            tr.AddRange(trs.Select(s => new TaskReport(s)));
        }

        public void FlagExpiration(DateTime now)
        {
            exp = now.Subtract(CompleteTS).TotalSeconds > expiration;
        }

        public override string ToString()
        {
            return $"ID:{id} IP:{ip} Ex:{Expired} Complete:{Complete}";
        }
    }
}