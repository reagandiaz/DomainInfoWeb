using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using DomainInfoCore;

namespace DomainInfoService.Background
{
    public class DomainInfoHostedService : IHostedService
    {
        //private Timer _timer;
        public static DomainInfoEngine Engine;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //starts the engine
                Engine = new DomainInfoEngine();
                Engine.Run();
            }
            catch
            {
                Clear();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Clear();
            return Task.CompletedTask;
        }

        void Clear()
        {
            Engine.Stop();
        }
    }
}
