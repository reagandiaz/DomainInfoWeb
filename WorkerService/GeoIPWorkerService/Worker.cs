using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using WorkerCore;

namespace GeoIPWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Handler handler;

        public Worker(ILogger<Worker> logger)
        {
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _logger = logger;
            handler = new Handler("http://localhost:1108/Worker", name.Substring(0, name.Length - ("WorkerService").Count()), logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    handler.Request();
                });

                List<WorkerQueueItem> toprocess = new List<WorkerQueueItem>();

                lock (handler.queue)
                {
                    if (handler.queue.Count > 0)
                    {
                        toprocess.AddRange(handler.queue.ToList());
                        handler.queue.Clear();
                    }
                }

                toprocess.ForEach(s =>
                {
                    Task.Run(() =>
                    {
                        WorkerReportItem repitem = handler.Process(s);
                        lock (handler.report)
                        {
                            handler.report.Add(repitem);
                        }
                    });
                });

                var toupload = new List<WorkerReportItem>();

                lock (handler.report)
                {
                    if (handler.report.Count > 0)
                    {
                        toupload.AddRange(handler.report.ToList());
                        handler.report.Clear();
                    }
                }

                if (toupload.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        handler.ReportToAPI(toupload);
                        handler.Request();
                    });
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}