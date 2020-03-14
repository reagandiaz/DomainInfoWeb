using System;
using System.Linq;
using System.Threading.Tasks;
using DomainInfoService.Background;
using Microsoft.AspNetCore.Mvc;

namespace DomainInfoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        [HttpPut]
        public Models.RequestState CreateIPDomainInfoRequest(Models.CreateRequest data)
        {
            Models.RequestState state = new Models.RequestState() { ip = data.ip };
            try
            {
                if (data.ip == null)
                {
                    state.message = "Error:ip can't be null";
                    return state;
                }
                System.Net.IPAddress address;

                //if not valid ip 
                var req = new DomainInfoCore.DataObject.IPRequest(data.ip, data.tasks);

                if (!System.Net.IPAddress.TryParse(data.ip, out address))
                {
                    if (Uri.CheckHostName(data.ip) == UriHostNameType.Unknown)
                    {
                        state.message = "Error:incorrect ip format";
                        return state;
                    }
                }

                if (req.TaskItems.Count == 0)
                {
                    state.message = "Error:no task to perform";
                    return state;
                }

                state.id = req.ID;
                state.message = "Success:queued";

                //loads the ip to queue
                Task.Run(() =>
                {
                    lock (DomainInfoHostedService.Engine.Cache.Requests)
                    {
                        DomainInfoHostedService.Engine.Cache.Requests.Add(req);
                    }
                });

            }
            catch (Exception ex)
            {
                DomainInfoHostedService.Engine.Cache.Logger.StampEx(ex);
                state.message = $"Error:{ex.Message}";
                return state;
            }
            return state;
        }

        [HttpPost]
        public Models.Report GetReport(Models.ReportRequest qp)
        {
            Models.Report resp = new Models.Report() { id = qp.Id };
            try
            {
                lock (DomainInfoHostedService.Engine.Cache.Reports)
                {
                    var match = DomainInfoHostedService.Engine.Cache.Reports.SingleOrDefault(s => s.ID == qp.Id);
                    if (match == null)
                    {
                        resp.message = "Message: no partial result yet";
                        return resp;
                    }
                    if (qp.Getpartial)
                    {
                        resp.Load(match);
                        resp.message = match.Complete ? "Message: Complete!" : $"Message: {(resp.reports == null ? 0 : match.TaskReports.Count)} partial result";
                        return resp;
                    }
                    if (match.Complete)
                    {
                        resp.Load(match);
                        resp.message = "Message: Complete!";
                        return resp;
                    }
                    resp.message = $"Message: {(match.TaskReports.Count)} partial result";
                }
            }
            catch (Exception ex)
            {
                DomainInfoHostedService.Engine.Cache.Logger.StampEx(ex);
                resp.message = $"Error:{ex.Message}";
            }
            return resp;
        }

    }
}