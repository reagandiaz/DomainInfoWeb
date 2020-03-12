using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace DomainInfoCore.Tasks
{
    public class Compile : basetask
    {
        public Compile(Cache cache) : base(cache) { }

        public static ICollection CreatTaskQueueItems(object cache)
        {
            var rawresult = ((DomainInfoCore.Cache)cache).PurgeRawResult();
            List<IPResult> queue = new List<IPResult>();
            if (rawresult.Count > 0)
            {
                rawresult.GroupBy(s => s.TaskQueueSource.ID).ToList().ForEach(s =>
                {
                    var res = s.ToList();
                    var ipres = new IPResult(s.Key, res[0].TaskQueueSource.IP, res[0].TaskQueueSource.ReportCount);
                    ipres.AddResults(res);
                    queue.Add(ipres);
                });
            }
            return queue;
        }

        public override void TaskExecute(ICollection queue)
        {
            ThreadPool.QueueUserWorkItem(c =>
            {
                var resultcache = ((DomainInfoCore.Cache)Cache).Reports;
                lock (resultcache)
                {
                    var now = DateTime.Now;
                    //clean up based on timeout
                    resultcache.Where(s => s.Complete && !s.Expired).ToList().ForEach(s =>
                    {
                        s.FlagExpiration(DateTime.Now);
                    });

                    var expired = resultcache.Where(s => s.Expired).ToList();
                    if (expired.Count > 0)
                        resultcache.RemoveAll(s => expired.Contains(s));

                    //merge to resultcache
                    var newdata = (List<IPResult>)queue;
                    var joinsel = (from t1 in resultcache
                                   join t2 in newdata on t1.ID equals t2.ID
                                   select new { existing = t1, update = t2 }).ToList();
                    if (joinsel.Count > 0)
                        joinsel.ForEach(s => { s.existing.TaskReports.AddRange(s.update.TaskReports); });

                    //add new
                    var newsel = (from t1 in newdata
                                  join t2 in resultcache on t1.ID equals t2.ID into temp
                                  from t2 in temp.DefaultIfEmpty()
                                  where t2 == null
                                  select t1).ToList();
                    resultcache.AddRange(newsel);

                    //flag complete
                    resultcache.Where(s => !s.Complete && s.ReportCount == s.TaskReports.Count).ToList().ForEach(s => { s.Complete = true; s.CompleteTS = DateTime.Now; });
                }
            });
        }
    }
}
