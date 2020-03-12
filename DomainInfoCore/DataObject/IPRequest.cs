using System;
using System.Collections.Generic;

namespace DomainInfoCore.DataObject
{
    public class IPRequest
    {
        static Int64 pkey;
        public Int64 ID => id;
        public string IP => ip;
        public DateTime TS => ts;
        public List<TaskItem> TaskItems => ti;
        public int ReportCount => rpcnt;

        readonly Int64 id;

        readonly DateTime ts;

        readonly string ip;

        readonly List<TaskItem> ti;

        readonly int rpcnt;

        public IPRequest(string ip, string[] tasks)
        {
            //autogenerate key
            id = ++pkey;
            this.ip = ip;
            ts = DateTime.Now;
            ti = new List<TaskItem>();
            for (int i = 0; i < tasks.Length; i++)
            {
                try
                {
                    TaskType ttype;
                    Enum.TryParse(tasks[i], out ttype);

                    if (Enum.GetName(ttype.GetType(), ttype) == tasks[i])
                        ti.Add(new TaskItem() { TaskType = ttype, TaskState = TaskState.New });
                }
                catch
                {
                    //just don't create it
                }
            }
            rpcnt = ti.Count;
        }
    }
}
