using System;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Orchard.Caching.Services;

namespace Associativy.Services
{
    public class GraphCacheService : IGraphCacheService
    {
        private readonly IGraphEventMonitor _graphEventMonitor;
        private readonly ICacheService _cacheService;


        public GraphCacheService(IGraphEventMonitor graphEventMonitor, ICacheService cacheService)
        {
            _graphEventMonitor = graphEventMonitor;
            _cacheService = cacheService;
        }


        public T GetMonitored<T>(IGraphDescriptor descriptor, string key, Func<T> factory)
        {
            return _cacheService.Get(key, () =>
                {
                    _graphEventMonitor.MonitorChanged(descriptor, key);
                    return factory();
                });
        }
    }
}