using System;
using System.Collections.Generic;
using System.Text;
using IntegrationTools;

namespace CoreDefinition.Task
{
    public class basecache
    {
        protected readonly Logger logger;
        public Logger Logger => logger;

        public basecache(string  path)
        {
            this.logger = new Logger(path);
        }
    }
}
