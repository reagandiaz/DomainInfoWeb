using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;

namespace DomainInfoCore.Tasks
{
    public class AssignRequest : basetask
    {
        DomainInfoCore.Cache cache;
        public AssignRequest(Cache cache)
        {
            this.cache = cache;
        }
        public override ICollection GetQueue()
        {
            List<TaskQueueItem> queue = new List<TaskQueueItem>();
            lock (cache.Requests)
            {
                if (cache.Requests.Count > 0)
                {
                    cache.Requests.ForEach(req =>
                    {
                        req.TaskItems.ForEach(ti =>
                        {
                            queue.Add(new TaskQueueItem(req, ti.TaskType));
                            ti.TaskState = DataObject.TaskState.Processing;
                        });
                    });
                    //since queue is made
                    cache.Requests.Clear();
                }
            }
            return queue;
        }

        public override void TaskExecute(ICollection queue)
        {
            Task.Run(() =>
            {
                lock (cache.TaskQueue)
                {
                    cache.TaskQueue.AddRange((List<TaskQueueItem>)queue);
                }
            });
        }
    }
}
