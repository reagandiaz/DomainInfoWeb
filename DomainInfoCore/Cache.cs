using System.Collections.Generic;
using System.Linq;
using CoreDefinition;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using DomainInfoCore.Tasks;

namespace DomainInfoCore
{
    public class Cache : basecache
    {       
        List<basetask> tasks;

        public List<basetask> Tasks => tasks;
       
        readonly List<IPRequest> requests = new List<IPRequest>();
        readonly List<TaskResultItem> taskreports = new List<TaskResultItem>();
        readonly List<IPResult> reports = new List<IPResult>();
        List<TaskQueueItem> taskqueue = new List<TaskQueueItem>();

        public List<IPRequest> Requests => requests;
        public List<TaskResultItem> TaskReports => taskreports;
        public List<IPResult> Reports => reports;
        public List<TaskQueueItem> TaskQueue => taskqueue;

        public Cache(string path): base(path)
        {
            requests = new List<IPRequest>();
            taskreports = new List<TaskResultItem>();
            reports = new List<IPResult>();
            LoadTask();
        }

        bool LoadTask()
        {
            tasks = new List<basetask>()
            {
                /*routines*/
                new AssignRequest(this){
                    Frequency = 5,
                    GetQueueNoThreshold = AssignRequest.CreatTaskQueueItems,
                    },
                new Compile(this){
                    Frequency = 5,
                    GetQueueNoThreshold = Compile.CreatTaskQueueItems,
                    },
            };

            return tasks.Count > 0;
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