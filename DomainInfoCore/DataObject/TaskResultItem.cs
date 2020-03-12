using System;
using System.Collections.Generic;
using System.Text;

namespace DomainInfoCore.DataObject
{
    public class TaskResultItem
    {
        TaskQueueItem tqitem;
        DateTime ts;

        public TaskQueueItem TaskQueueSource => tqitem;

        public DateTime TS => ts;

        public string Message { get; set; }

        public TaskState State { get; set; }

        public TaskResultItem(TaskQueueItem tq)
        {
            //completed
            ts = DateTime.Now;
            tqitem = tq;
        }
    }
}
