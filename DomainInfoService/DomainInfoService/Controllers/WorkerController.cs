using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainInfoCore.DataObject;
using DomainInfoService.Background;
using DomainInfoService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DomainInfoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : Controller
    {
        [HttpPut]
        public List<WorkerQueueItem> GetQueue(string task)
        {
            List<WorkerQueueItem> queue = new List<WorkerQueueItem>();
            try
            {
                lock (DomainInfoHostedService.Engine.Cache.TaskQueue)
                {
                    TaskType ttype;
                    Enum.TryParse(task, out ttype);
                    if (Enum.GetName(ttype.GetType(), ttype) == task)
                    {
                        var items = DomainInfoHostedService.Engine.Cache.TaskQueue.Where(s => s.TaskType == ttype).ToList();
                        if (items.Count > 0)
                        {
                            items.ForEach(s => { queue.Add(new WorkerQueueItem(s)); });
                            DomainInfoHostedService.Engine.Cache.TaskQueue.RemoveAll(s => items.Contains(s));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}: {ex.StackTrace}");
            }
            return queue;
        }

        [HttpPost]
        public void LoadReports(string task, List<WorkerReportItem> items)
        {
            try
            {
                List<TaskResultItem> tri = new List<TaskResultItem>();

                TaskType ttype;
                Enum.TryParse(task, out ttype);

                items.ForEach(s =>
                {
                    TaskState state;
                    Enum.TryParse(s.state, out state);
                    tri.Add(new TaskResultItem
                    {
                        ID = s.id,
                        IP = s.ip,
                        TaskType = ttype,
                        Data = s.data,
                        State = state,
                        ReportCount = s.rprtcnt,
                        QTS = s.qts,
                        TS = s.ts
                    }); ;
                });

                Task.Run(() =>
                {
                    lock (DomainInfoHostedService.Engine.Cache.TaskReports)
                    {
                        DomainInfoHostedService.Engine.Cache.TaskReports.AddRange(tri);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}: {ex.StackTrace}");
            }
        }
    }
}