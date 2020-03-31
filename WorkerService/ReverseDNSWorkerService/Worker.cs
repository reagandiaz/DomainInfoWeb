using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using WorkerCore.OpenAPIs;
using WorkerCore.DomainInfo;
using IntegrationTools;

namespace ReverseDNSWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Handler handler;
        private readonly int threadcount;

        public Worker(ILogger<Worker> logger)
        {
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _logger = logger;
            var openapiconfig = (new ConfigTool.ConfigReader<openapiconfig>("openapiconfig.json", new openapiconfig() { config = new List<hostconfig>() })).Config;
            var domainInfo = openapiconfig.config.SingleOrDefault(s => s.host == "DomainInfo");
            var url = domainInfo == null ? "http://localhost:1111" : domainInfo.url;
            threadcount = domainInfo == null ? 5 : domainInfo.thread;
            handler = new Handler($"{url}", name.Substring(0, name.Length - ("WorkerService").Count()), logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await handler.Request();

                List<WorkerQueueItem> toprocess = new List<WorkerQueueItem>();

                if (handler.queue.Count > 0)
                {
                    toprocess.AddRange(handler.queue.ToList());
                    handler.queue.Clear();
                }

                var batches = QueueHelper.CreateQueue<WorkerQueueItem>(toprocess, threadcount);

                if (batches.Count > 0)
                {
                    Task<List<WorkerReportItem>>[] tasks = new Task<List<WorkerReportItem>>[batches.Count];
                    for (int i = 0; i < tasks.Length; i++)
                        tasks[i] = GetReportItems(handler, batches[i]);

                    Task.WaitAll(tasks);

                    handler.report.AddRange((tasks.Select(s => s.Result).ToList()).SelectMany(s => s));
                }

                var toupload = new List<WorkerReportItem>();

                if (handler.report.Count > 0)
                {
                    toupload.AddRange(handler.report.ToList());
                    handler.report.Clear();
                }

                if (toupload.Count > 0)
                {
                    await handler.ReportToAPI(toupload);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        Task<List<WorkerReportItem>> GetReportItems(Handler handler, List<WorkerQueueItem> toprocess)
        {
            List<WorkerReportItem> reports = new List<WorkerReportItem>();
            toprocess.ForEach(s => { WorkerReportItem repitem = handler.Process(s); reports.Add(repitem); });
            return Task.FromResult<List<WorkerReportItem>>(reports);
        }
    }
}