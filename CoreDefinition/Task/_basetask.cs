using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using IntegrationTools;

namespace CoreDefinition.Task
{
    public abstract class basetask
    {
        protected readonly basecache cache;
        protected readonly Logger logger;
        protected readonly DateTime start;
        protected volatile int queuecnt;

        public delegate ICollection GetQueueCallback(int maxitemcnt);

        public delegate ICollection GetQueueCallbackNoThreshold(basecache cache);

        public int Frequency { get; set; }
        public int Threshold { get; set; }
        public basecache Cache => cache;

        public GetQueueCallback GetQueue { get; set; }

        public GetQueueCallbackNoThreshold GetQueueNoThreshold { get; set; }

        public abstract void TaskExecute(ICollection queue);

        protected basetask(basecache cache)
        {
            this.cache = cache;
            logger = cache.Logger;
            start = DateTime.Now;
        }

        public void Execute()
        {
            if (start.AddSeconds(Frequency) <= DateTime.Now)
            {
                if (GetQueueNoThreshold != null)
                {
                    var queue = GetQueueNoThreshold(this.cache);
                    if (queue.Count > 0)
                        TaskExecute(queue);
                }
                else
                {
                    int maxqueuecnt = Threshold - queuecnt;
                    if (maxqueuecnt > 0)
                    {
                        ICollection toprocess = GetQueue(maxqueuecnt);
                        TaskExecute(toprocess);
                    }
                }
            }
        }
    }
}
