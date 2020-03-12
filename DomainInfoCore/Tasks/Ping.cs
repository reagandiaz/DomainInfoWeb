using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;
using IntegrationTools;

namespace DomainInfoCore.Tasks
{
    public class Ping : TaskProcessTemplate
    {
        public Ping(Cache cache, TaskType tasktype) : base(cache, tasktype) { }

        public override TaskResultItem ExecuteProcess(TaskQueueItem queue)
        {
            /*nuget System.NET.Ping*/
            var result = new TaskResultItem(queue);

            StringBuilder sb = new StringBuilder();
            try
            {
                string data = "Hello! This is Reagan Diaz";
                var pingSender = new System.Net.NetworkInformation.Ping();
                PingOptions options = new PingOptions
                {
                    DontFragment = true
                };
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                const int timeout = 1024;
                PingReply reply = pingSender.Send(queue.IP, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    sb.Append($"Address: {reply.Address} ");
                    sb.Append($"RoundTrip: {reply.RoundtripTime} ");
                    sb.Append($"TimeTolive: {reply.Options.Ttl} ");
                    sb.Append($"DontFragment: {reply.Options.DontFragment} ");
                    sb.Append($"BufferSize: {reply.Buffer.Length}");
                }
                else
                    sb.Append(reply.Status);

                result.State = TaskState.Complete;
            }
            catch (Exception ex)
            {
                result.State = TaskState.Error;
                sb.Append(ex.Message);
            }
            result.Message = sb.ToString();
            return result;
        }
    }

}
