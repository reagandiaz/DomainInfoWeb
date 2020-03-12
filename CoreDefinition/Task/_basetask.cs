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
        protected readonly Object _cache;
        protected readonly Logger _logger;
        protected readonly DateTime _start;
        protected volatile int queuecnt;

        public delegate ICollection GetQueueCallback(int maxitemcnt);

        public delegate ICollection GetQueueCallbackNoThreshold(object cache);

        public int Frequency { get; set; }
        public int Threshold { get; set; }
        public Object Cache => _cache;

        public GetQueueCallback GetQueue { get; set; }

        public GetQueueCallbackNoThreshold GetQueueNoThreshold { get; set; }

        public abstract void TaskExecute(ICollection queue);

        protected basetask(object cache)
        {
            _cache = cache;
            _logger = ((ILogger)cache).Logger;
            _start = DateTime.Now;
        }

        public void Execute()
        {
            if (_start.AddSeconds(Frequency) <= DateTime.Now)
            {
                if (GetQueueNoThreshold != null)
                {
                    var queue = GetQueueNoThreshold(Cache);
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
