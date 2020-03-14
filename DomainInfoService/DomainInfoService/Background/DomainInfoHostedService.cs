using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DomainInfoCore;

namespace DomainInfoService.Background
{
    public class DomainInfoHostedService : IHostedService
    {
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
