using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;
using CoreDefinition.Task;

namespace CoreDefinition
{
    public abstract class Engine : IDisposable
    {
        System.Timers.Timer tasktimer;
        const int refresh = 1000;
        bool isbusy;
        public List<basetask> tasks;

        void TaskTick(object sender, ElapsedEventArgs e)
        {
            if (!isbusy)
            {
                isbusy = true;
                tasks.ForEach(x => x.Execute());
                isbusy = false;
            }
        }

        public abstract void Initialize();

        public void Run()
        {
            Initialize();
            (new Thread(new ThreadStart(() =>
            {
                tasktimer = new System.Timers.Timer(refresh);
                tasktimer.Elapsed += new ElapsedEventHandler(TaskTick);
                tasktimer.Enabled = true;
            }))).Start();
        }

        public void Stop()
        {
            if (tasktimer != null)
                tasktimer.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (tasktimer != null)
                {
                    tasktimer.Close();
                    tasktimer.Dispose();
                }
            }
        }
    }
}
