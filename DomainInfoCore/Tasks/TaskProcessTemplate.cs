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
    public abstract class TaskProcessTemplate : basetask
    {
        List<TaskQueueItem> queue = new List<TaskQueueItem>();
        List<TaskResultItem> result = new List<TaskResultItem>();
        readonly TaskType ttype;

        public TaskType TaskType => ttype;

        public TaskProcessTemplate(Cache cache, TaskType tasktype) : base(cache)
        {
            ttype = tasktype;
            GetQueue = CreateTaskQueueItems;
        }

        public abstract TaskResultItem ExecuteProcess(TaskQueueItem queue);

        public ICollection CreateTaskQueueItems(int maxitemcnt)
        {
            List<TaskQueueItem> toprocess = new List<TaskQueueItem>();
            var queue = ((TaskProcessTemplate)this).queue;

            lock (queue)
            {
                //just get below the threshold
                var items = queue.Take(maxitemcnt).ToList();
                if (items.Count > 0)
                {
                    int remove = Math.Min(queue.Count, items.Count);
                    queue.RemoveRange(0, remove);
                    toprocess.AddRange(items);
                    queuecnt += toprocess.Count;
                }
            }
            return toprocess;
        }

        public override void TaskExecute(ICollection queue)
        {
            List<TaskQueueItem> toprocess = (List<TaskQueueItem>)queue;
            toprocess.ForEach(s =>
            {
                ThreadPool.QueueUserWorkItem(c =>
                {
                    TaskResultItem item = ExecuteProcess(s);
                    lock (result)
                    {
                        result.Add(item);
                        queuecnt--;
                    }
                 
                });
            });
        }

        public void AddQueue(List<TaskQueueItem> items)
        {
            lock (queue)
            {
                queue.AddRange(items);
            }
        }

        public List<TaskResultItem> PurgeResult()
        {
            List<TaskResultItem> items = new List<TaskResultItem>();
            lock (result)
            {
                if (result.Count > 0)
                {
                    //move all
                    items.AddRange(result);
                    result.Clear();
                }
            }
            return items;
        }
    }
}


