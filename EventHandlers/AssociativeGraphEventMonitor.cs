using System.Collections.Concurrent;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.Caching;

namespace Associativy.Services
{
    public class AssociativeGraphEventMonitor : IAssociativeGraphEventMonitor
    {
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        /// <summary>
        /// Stores the objects that were fed as signals.
        /// </summary>
        /// <remarks>
        /// Using a static field is maybe not the most elegant way, but it works.
        /// It's crucial that this bag stays alive at least as long as the ISignals instance. Since ISignals is an 
        /// ISingletonDependency this field could store an ISingletonDependency as well, but statics, just as ISingletonDependencys
        /// live as long as the shell.
        /// </remarks>
        private static ConcurrentDictionary<string, string> _changedSignals = new ConcurrentDictionary<string, string>();

        public AssociativeGraphEventMonitor(
            ICacheManager cacheManager,
            ISignals signals)
        {
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public void MonitorChanged(IAcquireContext aquireContext, IAssociativyContext associativyContext)
        {
            var signal = associativyContext.TechnicalName + "ChangedSignal";
            _changedSignals[associativyContext.TechnicalName] = signal;
            aquireContext.Monitor(_signals.When(signal));
        }

        public void Changed(IAssociativyContext associativyContext)
        {
            string signal;
            if (_changedSignals.TryGetValue(associativyContext.TechnicalName, out signal))
            {
                _signals.Trigger(signal);
            }
        }
    }
}