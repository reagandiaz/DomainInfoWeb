using CoreDefinition;
using System.Reflection;

namespace DomainInfoCore
{
    public class DomainInfoEngine : Engine
    {
        Cache cache;
        public Cache Cache => cache;

        public override void Initialize()
        {
            cache = new Cache((string.Format("{0}//{1}", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_logs")));
            tasks = cache.Tasks;
        }
    }
}
