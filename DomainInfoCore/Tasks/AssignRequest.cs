using CoreDefinition.Task;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DomainInfoCore.DataObject;
using IntegrationTools;
using System.Threading;

namespace DomainInfoCore.Tasks
{
    public class AssignRequest : basetask
    {
        public AssignRequest(Cache cache) : base(cache) { }

        public static ICollection CreatTaskQueueItems(object cache)
        {
            List<TaskQueueItem> queue = new List<TaskQueueItem>();
            var iprequest = (((DomainInfoCore.Cache)cache).Requests);
            lock (iprequest)
            {
                if (iprequest.Count > 0)
                {
                    if (iprequest.Count > 0)
                    {
                        iprequest.ForEach(req =>
                        {
                            req.TaskItems.ForEach(ti =>
                            {
                                queue.Add(new TaskQueueItem(req, ti.TaskType));
                                ti.TaskState = DataObject.TaskState.Processing;
                            });
                        });
                        //since queue is made
                        iprequest.Clear();
                    }
                }
            }
            return queue;
        }

        public override void TaskExecute(ICollection queue)
        {
            ((List<TaskQueueItem>)queue).GroupBy(s => s.TaskType).ToList().ForEach(s =>
            {
                var tasktable = ((DomainInfoCore.Cache)Cache).TaskProcesses;
                ThreadPool.QueueUserWorkItem(c =>
                {
                    var task = tasktable.SingleOrDefault(t => s.Key == ((TaskProcessTemplate)t).TaskType);
                    if (task != null)
                        ((TaskProcessTemplate)task).AddQueue(s.ToList());
                });
            });
        }
    }
}
