using Associativy.GraphDiscovery;

namespace Associativy.EventHandlers
{
    public interface IGraphEventMonitor : IGraphEventHandler
    {
        void MonitorChanged(IGraphDescriptor graphDescriptor, string cacheKey);
    }
}
