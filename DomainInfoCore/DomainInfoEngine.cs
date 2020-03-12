using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreDefinition;
using DomainInfoCore.Tasks;
using CoreDefinition.Task;

namespace DomainInfoCore
{
    public class DomainInfoEngine : Engine
    {
        Cache cache;

        public Cache Cache => cache;

        public override void Initialize()
        {
            cache = new Cache();
            tasks = cache.Tasks;  
        }

    }
}
