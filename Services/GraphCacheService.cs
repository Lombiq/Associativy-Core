using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Orchard.Caching.Services;

namespace Associativy.Services
{
    public class GraphCacheService : IGraphCacheService
    {
        private readonly IGraphEventMonitor _graphEventMonitor;
        private readonly ICacheService _cacheService;
        private readonly HashSet<string> _disabledGraphNames;


        public GraphCacheService(IGraphEventMonitor graphEventMonitor, ICacheService cacheService)
        {
            _graphEventMonitor = graphEventMonitor;
            _cacheService = cacheService;
            _disabledGraphNames = new HashSet<string>();
        }


        public T GetMonitored<T>(IGraphDescriptor descriptor, string key, Func<T> factory)
        {
            if (_disabledGraphNames.Contains(descriptor.Name)) return factory();

            return _cacheService.Get(key, () =>
                {
                    _graphEventMonitor.MonitorChanged(descriptor, key);
                    return factory();
                });
        }


        public void SetEnabledStateForRequest(IGraphDescriptor descriptor, bool isEnabled)
        {
            var graphName = descriptor.Name;

            if (!isEnabled) _disabledGraphNames.Add(graphName);
            else if (_disabledGraphNames.Contains(graphName)) _disabledGraphNames.Remove(graphName);
        }
    }
}