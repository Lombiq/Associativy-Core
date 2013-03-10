using System.Collections.Concurrent;
using Associativy.GraphDiscovery;
using Orchard;
using Orchard.Caching;
using Orchard.Caching.Services;

namespace Associativy.EventHandlers
{
    public class GraphEventMonitor : GraphEventHandlerBase, IGraphEventMonitor
    {
        private const string KeyChainCacheKey = "Associativy.GraphEventMonitor.KeyChain";

        private readonly ICacheService _cacheService;


        public GraphEventMonitor(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }


        public void MonitorChanged(IGraphDescriptor graphDescriptor, string cacheKey)
        {
            GetKeys().TryAdd(cacheKey, 0);
        }

        public override void Changed(IGraphDescriptor graphDescriptor)
        {
            foreach (var keyKvp in GetKeys())
            {
                _cacheService.Remove(keyKvp.Key);
            }
        }


        private ConcurrentDictionary<string, byte> GetKeys()
        {
            return _cacheService.Get(KeyChainCacheKey, () =>
                        {
                            return new ConcurrentDictionary<string, byte>();
                        });
        }
    }
}