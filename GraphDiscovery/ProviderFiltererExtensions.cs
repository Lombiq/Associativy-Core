using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public static class ProviderFiltererExtensions
    {
        public static IEnumerable<IGraphProvider> FilterByMatchingGraphContext(
            this IProviderFilterer providerFilterer, 
            IEnumerable<IGraphProvider> providers, 
            IGraphContext graphContext)
        {
            return providerFilterer.FilterByMatchingGraphContext(providers, graphContext, (provider) => provider.GraphName, (provider) => provider.ContentTypes);
        }
    }
}