using Associativy.Models;
using Orchard.Caching;
using Associativy.GraphDiscovery;

namespace Associativy.EventHandlers
{
    public interface IGraphEventMonitor : IGraphEventHandler
    {
        void MonitorChanged(IGraphContext graphContext, IAcquireContext acquireContext);
    }
}
