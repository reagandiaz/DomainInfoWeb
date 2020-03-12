using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainInfoService.Models
{
    public class RequestState
    {
        public Int64 id { get; set; }
        public string ip { get; set; }
        public string message { get; set; }
    }
}