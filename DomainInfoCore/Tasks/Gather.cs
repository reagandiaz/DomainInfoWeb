using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using IntegrationTools;

namespace DomainInfoCore.Tasks
{
    public class Gather : basetask
    {
        public Gather(Cache cache) : base(cache) { }

        public static ICollection CreatTaskQueueItems(basecache cache)
        {
            List<TaskResultItem> items = new List<TaskResultItem>();

            using (CountdownEvent e = new CountdownEvent(1))
            {
                foreach (var q in ((DomainInfoCore.Cache)cache).TaskProcesses)
                {
                    // Dynamically increment signal count.
                    e.AddCount();
                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        try
                        {
                            //compile from each of the task
                            var res = ((TaskProcessTemplate)q).PurgeResult();
                            if (res.Count > 0)
                            {
                                lock (items)
                                {
                                    items.AddRange(res);
                                }
                            }
                        }
                        finally
                        {
                            e.Signal();
                        }
                    });
                }
                e.Signal();
                // The first element could be run on this thread. 

                // Join with work.
                e.Wait();
            }
            return items;
        }

        public override void TaskExecute(ICollection queue)
        {
            var resultcache = ((DomainInfoCore.Cache)Cache).TaskReports;
            ThreadPool.QueueUserWorkItem(c =>
            {
                lock (resultcache)
                {
                    resultcache.AddRange((List<TaskResultItem>)queue);
                }
            });
        }
    }
}
