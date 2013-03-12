using System;
using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.Services
{
    public interface IGraphCacheService : IDependency
    {
        T GetMonitored<T>(IGraphDescriptor descriptor, string key, Func<T> factory);
    }

    public static class GraphCachingServiceExtensions
    {
        public static T GetMonitored<T>(this IGraphCacheService cachingService, IGraphDescriptor descriptor, string key, Func<T> factory, bool useCache)
        {
            if (useCache) return cachingService.GetMonitored(descriptor, key, factory);
            return factory();
        }
    }
}
