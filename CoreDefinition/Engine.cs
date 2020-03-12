using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;
using CoreDefinition.Task;

namespace CoreDefinition
{
    public abstract class Engine : IDisposable
    {
        System.Timers.Timer _tasktimer;
        const int _TaskRefresh = 1000;
        bool _isbusy;
        public List<basetask> tasks;

        void TaskTick(object sender, ElapsedEventArgs e)
        {
            if (!_isbusy)
            {
                _isbusy = true;
                tasks.ForEach(x => x.Execute());
                _isbusy = false;
            }
        }

        public abstract void Initialize();

        public void Run()
        {
            Initialize();
            (new Thread(new ThreadStart(() =>
            {
                _tasktimer = new System.Timers.Timer(_TaskRefresh);
                _tasktimer.Elapsed += new ElapsedEventHandler(TaskTick);
                _tasktimer.Enabled = true;
            }))).Start();
        }

        public void Stop()
        {
            if (_tasktimer != null)
                _tasktimer.Stop();
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
                if (_tasktimer != null)
                {
                    _tasktimer.Close();
                    _tasktimer.Dispose();
                }
            }
        }
    }
}
