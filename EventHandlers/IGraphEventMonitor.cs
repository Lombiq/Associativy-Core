using Associativy.GraphDiscovery;
using Orchard.Caching;

namespace Associativy.EventHandlers
{
    public interface IGraphEventMonitor : IGraphEventHandler
    {
        void MonitorChanged(IGraphContext graphContext, IAcquireContext acquireContext);
    }
}
