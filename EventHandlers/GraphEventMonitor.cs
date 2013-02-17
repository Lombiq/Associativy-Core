using System.Collections.Concurrent;
using Associativy.GraphDiscovery;
using Orchard;
using Orchard.Caching;

namespace Associativy.EventHandlers
{
    public class GraphEventMonitor : GraphEventHandlerBase, IGraphEventMonitor
    {
        private readonly ISignals _signals;
        private readonly ISignalStorage _signalStorage;


        public GraphEventMonitor(ISignals signals, ISignalStorage signalStorage)
        {
            _signals = signals;
            _signalStorage = signalStorage;
        }


        public void MonitorChanged(IGraphContext graphContext, IAcquireContext acquireContext)
        {
            var signal = graphContext.GraphName + "ChangedSignal";
            _signalStorage.Signals[graphContext.GraphName] = signal;
            acquireContext.Monitor(_signals.When(signal));
        }

        public override void Changed(IGraphContext graphContext)
        {
            string signal;
            if (_signalStorage.Signals.TryGetValue(graphContext.GraphName, out signal))
            {
                _signals.Trigger(signal);
            }
        }
    }

    public interface ISignalStorage : ISingletonDependency
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
    public class SignalStorage : ISignalStorage
    {
        private readonly ConcurrentDictionary<string, string> _signals = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> Signals
        {
            get { return _signals; }
        }
    }
}