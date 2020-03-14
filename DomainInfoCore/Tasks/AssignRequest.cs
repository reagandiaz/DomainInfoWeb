using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;

namespace DomainInfoCore.Tasks
{
    public class AssignRequest : basetask
    {
        public AssignRequest(Cache cache) : base(cache) { }

        public override ICollection GetQueue(basecache cache)
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
            var taskqueue = ((DomainInfoCore.Cache)Cache).TaskQueue;
            Task.Run(() =>
            {
                lock (taskqueue)
                {
                    taskqueue.AddRange((List<TaskQueueItem>)queue);
                }
            });
        }
    }
}
