using System;
using Orchard.Caching;
using Associativy.Models;

namespace Associativy.EventHandlers
{
    public interface IAssociativeGraphEventMonitor : IAssociativeGraphEventHandler
    {
        void MonitorChanged(IAcquireContext aquireContext, IAssociativyContext associativyContext);
    }
}
