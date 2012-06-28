using System.Collections.Concurrent;
using Associativy.Models;
using Orchard.Caching;
using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.EventHandlers
{
    public class GraphEventMonitor : GraphEventHandlerBase, IGraphEventMonitor
    {
        private readonly ISignals _signals;
        private readonly ISignalStore _signalStore;

        public GraphEventMonitor(ISignals signals, ISignalStore signalStore)
        {
            _signals = signals;
            _signalStore = signalStore;
        }

        public void MonitorChanged(IGraphContext graphContext, IAcquireContext acquireContext)
        {
            var signal = graphContext.GraphName + "ChangedSignal";
            _signalStore.Signals[graphContext.GraphName] = signal;
            acquireContext.Monitor(_signals.When(signal));
        }

        public override void Changed(IGraphContext graphContext)
        {
            string signal;
            if (_signalStore.Signals.TryGetValue(graphContext.GraphName, out signal))
            {
                _signals.Trigger(signal);
            }
        }
    }

    public interface ISignalStore : ISingletonDependency
    {
        ConcurrentDictionary<string, string> Signals { get; }
    }

    /// <summary>
    /// Stores signal objects
    /// </summary>
    /// <remarks>
    /// It's crucial that this dictionary stays alive at least as long as the ISignals instance. ISignals is an 
    /// ISingletonDependency too.
    /// </remarks>
    public class SignalStore : ISignalStore
    {
        public ConcurrentDictionary<string, string> Signals { get; private set; }

        public SignalStore()
        {
            Signals = new ConcurrentDictionary<string, string>();
        }
    }
}