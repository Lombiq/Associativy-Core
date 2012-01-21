using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;
using System.Diagnostics;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphManager : IGraphManager
    {
        private readonly IEnumerable<IGraphProvider> _registeredProviders;
        private readonly IProviderFilterer _providerFilterer;

        public GraphManager(
            IEnumerable<IGraphProvider> registeredProviders,
            IProviderFilterer providerFilterer)
        {
            _registeredProviders = registeredProviders;
            _providerFilterer = providerFilterer;
        }

        public IGraphProvider FindLastProvider(IGraphContext graphContext)
        {
            return FindProviders(graphContext).LastOrDefault();
        }

        public IEnumerable<IGraphProvider> FindProviders(IGraphContext graphContext)
        {
            // That's very fast (~70 ticks), so there's no point in caching anything.
            // If it gets heavy, could be stored in an instance cache by context.

            return _providerFilterer.FilterByMatchingGraphContext(_registeredProviders, graphContext);
        }
    }
}