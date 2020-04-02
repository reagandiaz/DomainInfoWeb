using System.Collections.Generic;
using System.Linq;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using DomainInfoCore.Tasks;

namespace DomainInfoCore
{
    public class Cache
    {
        readonly List<basetask> tasks;
        readonly List<IPRequest> requests;
        readonly List<TaskResultItem> taskreports;
        readonly List<IPResult> reports;
        readonly List<TaskQueueItem> taskqueue;

        public List<basetask> Tasks => tasks;
        public List<IPRequest> Requests => requests;
        public List<TaskResultItem> TaskReports => taskreports;
        public List<IPResult> Reports => reports;
        public List<TaskQueueItem> TaskQueue => taskqueue;

        public Cache(string path)
        {
            requests = new List<IPRequest>();
            taskreports = new List<TaskResultItem>();
            reports = new List<IPResult>();
            taskqueue = new List<TaskQueueItem>();
            tasks = new List<basetask>()
            {
                /*routines*/
                new AssignRequest(this){
                    Frequency = 5,
                    },
                new Compile(this){
                    Frequency = 5,
                    },
            };
        }

        public List<TaskResultItem> PurgeRawResult()
        {
            List<TaskResultItem> items = new List<TaskResultItem>();
            lock (taskreports)
            {
                if (taskreports.Count > 0)
                {
                    //move all
                    items.AddRange(taskreports.ToList());
                    taskreports.Clear();
                }
            }
            return items;
        }
    }
}