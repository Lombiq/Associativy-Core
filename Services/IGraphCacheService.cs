using System;
using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.Services
{
    public interface IGraphCacheService : IDependency
    {
        /// <summary>
        /// Gets/sets a monitored cache entry that will expire once the underlying graph changes
        /// </summary>
        /// <param name="descriptor">The descriptor of the graph</param>
        /// <param name="key">Cache entry key</param>
        /// <param name="factory">Value factory of the cache entry</param>
        /// <returns>The newly set value if there was no valid cache entry present or the cached value</returns>
        T GetMonitored<T>(IGraphDescriptor descriptor, string key, Func<T> factory);

        /// <summary>
        /// Enables or disables caching for the current request
        /// </summary>
        /// <param name="descriptor">The descriptor of the graph to enable/disable caching for</param>
        /// <param name="isEnabled">The enabled state</param>
        void SetEnabledStateForRequest(IGraphDescriptor descriptor, bool isEnabled);
    }
}
