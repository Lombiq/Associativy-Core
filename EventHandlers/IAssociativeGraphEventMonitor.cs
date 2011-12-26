using System;
using Orchard.Caching;

namespace Associativy.EventHandlers
{
    public interface IAssociativeGraphEventMonitor : IAssociativeGraphEventHandler
    {
        void MonitorChangedSignal(IAcquireContext ctx, object signal);
    }
}
