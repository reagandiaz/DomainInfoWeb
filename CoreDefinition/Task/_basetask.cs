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
        DateTime lastrun;

        public abstract ICollection GetQueue(basecache cache);

        public int Frequency { get; set; }

        public basecache Cache => cache;

        public abstract void TaskExecute(ICollection queue);

        protected basetask(basecache cache)
        {
            this.cache = cache;
            logger = cache.Logger;
            lastrun = DateTime.MinValue;
        }

        public void Execute()
        {
            if (lastrun.AddSeconds(Frequency) <= DateTime.Now)
            {
                var queue = GetQueue(this.cache);
                if (queue.Count > 0)
                    TaskExecute(queue);
                lastrun = DateTime.Now;
            }
        }
    }
}

