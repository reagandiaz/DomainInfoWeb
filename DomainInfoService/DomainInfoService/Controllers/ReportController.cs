using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInfoService.Background;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                        resp.info = "No partial results yet";
                        return resp;
                    }
                    if (qp.Getpartial)
                    {
                        resp.Load(match);
                        resp.info = match.Complete ? "Complete!" : $"{(resp.reports == null ? 0 : match.TaskReports.Count)} partial result/s";
                        return resp;
                    }
                    if (match.Complete)
                    {
                        resp.Load(match);
                        resp.info = "Complete!";
                        return resp;
                    }
                    resp.info = $"{(match.TaskReports.Count)} partial result/s";
                }
            }
            catch (Exception ex)
            {
                resp.info = $"Error:{ex.Message}";
            }
            return resp;
        }

        [HttpGet]
        public async Task Get(int id)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Models.Report resp = new Models.Report() { id = id };
            int retry = 5;
            try
            {
                while (retry > 0)
                {
                    lock (DomainInfoHostedService.Engine.Cache.Reports)
                    {
                        var match = DomainInfoHostedService.Engine.Cache.Reports.SingleOrDefault(s => s.ID == id);
                        if (match == null)
                            resp.info = "No partial results yet";
                        else
                        {
                            resp.Load(match);
                            if (match.Complete)
                            {
                                resp.info = "Complete!";
                                retry = 0;
                            }
                            else
                                resp.info = $"{(resp.reports == null ? 0 : match.TaskReports.Count)} partial result/s";
                        }
                    }
                    byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(resp));
                    await Response.Body.WriteAsync(messageBytes, 0, messageBytes.Length);
                    await Response.Body.FlushAsync();
                   
                    if (retry == 0)
                        break;

                    await Task.Delay(TimeSpan.FromSeconds(3));
                    retry--;
                }
            }
            catch (Exception ex)
            {
                resp.info = $"Error:{ex.Message} : {ex.StackTrace}";
                byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(resp));
                await Response.Body.WriteAsync(messageBytes, 0, messageBytes.Length);
                await Response.Body.FlushAsync();
            }
        }
    }
}