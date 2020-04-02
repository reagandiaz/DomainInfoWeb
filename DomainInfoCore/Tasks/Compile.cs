using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DomainInfoCore.DataObject;
using System.Threading.Tasks;
using CoreDefinition.Task;

namespace DomainInfoCore.Tasks
{
    public class Compile : basetask
    {
        DomainInfoCore.Cache cache;
        public Compile(Cache cache)
        {
            this.cache = cache;
        }

        public override ICollection GetQueue()
        {
            var rawresult = cache.PurgeRawResult();
            List<IPResult> queue = new List<IPResult>();
            if (rawresult.Count > 0)
            {
                rawresult.GroupBy(s => s.ID).ToList().ForEach(s =>
                {
                    var res = s.ToList();
                    var ipres = new IPResult(s.Key, res[0].IP, res[0].ReportCount);
                    ipres.AddResults(res);
                    queue.Add(ipres);
                });
            }
            return queue;
        }

        public override void TaskExecute(ICollection queue)
        {
            Task.Run(() =>
            {
                lock (cache.Reports)
                {
                    var now = DateTime.Now;
                    //clean up based on timeout
                    cache.Reports.Where(s => s.Complete && !s.Expired).ToList().ForEach(s =>
                    {
                        s.FlagExpiration(DateTime.Now);
                    });

                    var expired = cache.Reports.Where(s => s.Expired).ToList();
                    if (expired.Count > 0)
                        cache.Reports.RemoveAll(s => expired.Contains(s));

                    //merge to resultcache
                    var newdata = (List<IPResult>)queue;
                    var joinsel = (from t1 in cache.Reports
                                   join t2 in newdata on t1.ID equals t2.ID
                                   select new { existing = t1, update = t2 }).ToList();
                    if (joinsel.Count > 0)
                        joinsel.ForEach(s => { s.existing.TaskReports.AddRange(s.update.TaskReports); });

                    //add new
                    var newsel = (from t1 in newdata
                                  join t2 in cache.Reports on t1.ID equals t2.ID into temp
                                  from t2 in temp.DefaultIfEmpty()
                                  where t2 == null
                                  select t1).ToList();
                    cache.Reports.AddRange(newsel);

                    //flag complete
                    cache.Reports.Where(s => !s.Complete && s.ReportCount == s.TaskReports.Count).ToList().ForEach(s => { s.Complete = true; s.CompleteTS = DateTime.Now; });
                }
            });
        }
    }
}
