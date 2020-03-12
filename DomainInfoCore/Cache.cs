using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreDefinition.Task;
using DomainInfoCore.Tasks;
using System.IO;
using System.Reflection;
using DomainInfoCore.DataObject;
using IntegrationTools;
using CoreDefinition;

namespace DomainInfoCore
{
    public class Cache : ILogger
    {
        public static readonly string Path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        readonly Logger logger;
        List<basetask> tasks;
        List<basetask> taskprocess;

        public Logger Logger => logger;
        public List<basetask> Tasks => tasks;
        public List<basetask> TaskProcesses => taskprocess;

        readonly List<IPRequest> requests = new List<IPRequest>();
        readonly List<TaskResultItem> taskreports = new List<TaskResultItem>();
        readonly List<IPResult> reports = new List<IPResult>();

        public List<IPRequest> Requests => requests;
        public List<TaskResultItem> TaskReports => taskreports;
        public List<IPResult> Reports => reports;

        public Cache()
        {
            this.logger = new Logger(string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_logs"));
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
                new Gather(this){
                    Frequency = 5,
                    GetQueueNoThreshold = Gather.CreatTaskQueueItems,
                    },
                new Compile(this){
                    Frequency = 5,
                    GetQueueNoThreshold = Compile.CreatTaskQueueItems,
                    },
                
                /*tasks*/
                new GeoIP(this, TaskType.GeoIP){
                    Frequency = 5,
                    Threshold = 10,
                    },
                new Ping(this, TaskType.Ping){
                    Frequency = 5,
                    Threshold = 10,
                    },
                new ReverseDNS(this, TaskType.ReverseDNS){
                    Frequency = 5,
                    Threshold = 10,
                    },
            };

            taskprocess = tasks.Where(s => s is TaskProcessTemplate).ToList();

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
                    items.AddRange(taskreports);
                    taskreports.Clear();
                }
            }
            return items;
        }

    }
}
