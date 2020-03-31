using System.Collections.Generic;

namespace WorkerCore.OpenAPIs
{
    public class hostconfig
    {
        public string host { get; set; }
        public string url { get; set; }
        public int thread { get; set; }
    }

    public class openapiconfig
    {
        public List<hostconfig> config { get; set; }
    }
}
