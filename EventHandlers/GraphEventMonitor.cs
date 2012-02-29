using System.Collections.Concurrent;
using Associativy.Models;
using Orchard.Caching;
using Associativy.GraphDiscovery;

namespace Associativy.EventHandlers
{
    public class GraphEventMonitor : GraphEventHandlerBase, IGraphEventMonitor
    {
        private readonly ISignals _signals;

        /// <summary>
        /// Stores signal objects
        /// </summary>
        /// <remarks>
        /// Using a static field is maybe not the most elegant way, but it works.
        /// It's crucial that this dictionary stays alive at least as long as the ISignals instance. Since ISignals is an 
        /// ISingletonDependency this field could store an ISingletonDependency as well, but statics, just as ISingletonDependencys
        /// live as long as the shell.
        /// </remarks>
        private static ConcurrentDictionary<string, string> _changedSignals = new ConcurrentDictionary<string, string>();

        public GraphEventMonitor(ISignals signals)
        {
            _signals = signals;
        }

        public void MonitorChanged(IGraphContext graphContext, IAcquireContext acquireContext)
        {
            var signal = graphContext.GraphName + "ChangedSignal";
            _changedSignals[graphContext.GraphName] = signal;
            acquireContext.Monitor(_signals.When(signal));
        }

        public override void Changed(IGraphContext graphContext)
        {
            string signal;
            if (_changedSignals.TryGetValue(graphContext.GraphName, out signal))
            {
                _signals.Trigger(signal);
            }
        }
    }
}