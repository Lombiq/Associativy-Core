using Associativy.Models;
using Orchard.Caching;
using Associativy.GraphDescription.Contexts;

namespace Associativy.EventHandlers
{
    public interface IGraphEventMonitor : IGraphEventHandler
    {
        void MonitorChanged(IAcquireContext aquireContext, IGraphContext graphContext);
    }
}
