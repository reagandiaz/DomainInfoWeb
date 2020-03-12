using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CoreDefinition.Task;
using DomainInfoCore.DataObject;

namespace DomainInfoCore.Tasks
{
    public class ReverseDNS : TaskProcessTemplate
    {
        public ReverseDNS(Cache cache, TaskType tasktype) : base(cache, tasktype) { }

        public override TaskResultItem ExecuteProcess(TaskQueueItem queue)
        {
        
            /*consume service here*/
            var result = new TaskResultItem(queue);

            try
            {
                System.Net.IPAddress address;
                if (System.Net.IPAddress.TryParse(queue.IP, out address))
                {
                    //an ip
                    result.Message = Dns.GetHostEntry(queue.IP).HostName;
                    result.State = TaskState.Complete;
                }
                else
                {
                    //domain
                    result.Message = Dns.GetHostAddresses(queue.IP)[0].ToString();
                    result.State = TaskState.Complete;
                }
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
                result.State = TaskState.Error;
            }
            return result;
        }
    }
}
