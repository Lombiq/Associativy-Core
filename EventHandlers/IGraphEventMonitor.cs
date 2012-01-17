using Associativy.Models;
using Orchard.Caching;

namespace Associativy.EventHandlers
{
    public interface IGraphEventMonitor : IGraphEventHandler
    {
        void MonitorChanged(IAcquireContext aquireContext, IGraphDescriptor graphDescriptor);
    }
}
