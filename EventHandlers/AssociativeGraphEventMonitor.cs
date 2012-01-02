﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.EventHandlers;
using Orchard.Caching;
using System.Collections.Concurrent;

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
        private static ConcurrentBag<object> _changedSignalObjects = new ConcurrentBag<object>();

        public AssociativeGraphEventMonitor(
            ICacheManager cacheManager,
            ISignals signals)
        {
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public void MonitorChangedSignal(IAcquireContext ctx, object signal/*, context*/)
        {
            _changedSignalObjects.Add(signal);
            ctx.Monitor(_signals.When(signal));
        }

        public void Changed()
        {
            foreach (var signal in _changedSignalObjects)
            {
                _signals.Trigger(signal); 
            }
        }
    }
}