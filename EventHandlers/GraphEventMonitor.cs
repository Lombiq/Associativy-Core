using System;
using System.Collections.Concurrent;
using System.Linq;
using Associativy.GraphDiscovery;
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
            var newDicionaryLazy = new Lazy<ConcurrentDictionary<string, byte>>(() =>
                {
                    var dictionary = new ConcurrentDictionary<string, byte>();
                    dictionary[cacheKey] = 0;
                    return dictionary;
                });

            GetKeys().AddOrUpdate(
                graphDescriptor.Name,
                newDicionaryLazy.Value,
                (key, dictionary) =>
                {
                    dictionary[cacheKey] = 0;
                    return dictionary;
                });
        }

        public override void Changed(IGraphDescriptor graphDescriptor)
        {
            ConcurrentDictionary<string, byte> dictionary;
            if (GetKeys().TryGetValue(graphDescriptor.Name, out dictionary))
            {
                byte dummy;

                foreach (var cacheKey in dictionary.Keys.ToList())
                {
                    _cacheService.Remove(cacheKey);
                    dictionary.TryRemove(cacheKey, out dummy);
                }
            }
        }


        private ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> GetKeys()
        {
            return _cacheService.Get(KeyChainCacheKey, () =>
                        {
                            return new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();
                        });
        }
    }
}