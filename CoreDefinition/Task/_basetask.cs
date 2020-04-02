using System;
using System.Collections;
using IntegrationTools;

namespace CoreDefinition.Task
{
    public abstract class basetask
    {
        DateTime lastrun;

        public abstract ICollection GetQueue();

        public int Frequency { get; set; }

        public abstract void TaskExecute(ICollection queue);

        protected basetask()
        {
            lastrun = DateTime.MinValue;
        }

        public void Execute()
        {
            if (lastrun.AddSeconds(Frequency) <= DateTime.Now)
            {
                var queue = GetQueue();
                if (queue.Count > 0)
                    TaskExecute(queue);
                lastrun = DateTime.Now;
            }
        }
    }
}

