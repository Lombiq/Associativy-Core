using Associativy.Models;
using Orchard.Caching;

namespace Associativy.EventHandlers
{
    public interface IAssociativeGraphEventMonitor : IAssociativeGraphEventHandler
    {
        void MonitorChanged(IAcquireContext aquireContext, IAssociativyContext associativyContext);
    }
}
