using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class ProviderFilterer : IProviderFilterer
    {
        public IEnumerable<TAssociativyProvider> FilterByMatchingGraphContext<TAssociativyProvider>(
            IEnumerable<TAssociativyProvider> providers, 
            IGraphContext graphContext, 
            Func<TAssociativyProvider, string> graphNameSelector, 
            Func<TAssociativyProvider, IEnumerable<string>> contentTypesSelector) 
            where TAssociativyProvider : IAssociativyProvider
        {
            var filteredProviders = providers;

            if (!String.IsNullOrEmpty(graphContext.GraphName))
            {
                filteredProviders = filteredProviders.Where((provider) => graphNameSelector(provider) == graphContext.GraphName);
            }


            if (graphContext.ContentTypes != null && graphContext.ContentTypes.Count() != 0)
            {
                if (filteredProviders.Count() == 0)
                {
                    // If there are no providers with the graph context, try only with content types
                    filteredProviders = providers;
                }

                filteredProviders = filteredProviders.Where((provider) =>
                {

                    foreach (var contentType in graphContext.ContentTypes)
                    {
                        // A provider is only suitable if it has no content types specified (catch-all) or contains all content 
                        // types listed in the context.
                        if (contentTypesSelector(provider) == null || contentTypesSelector(provider).Count() == 0) return true;
                        if (!contentTypesSelector(provider).Contains(contentType)) return false;
                    }

                    return true;
                });

                // Catch-alls will be at the top, so the more specific providers at the bottom.
                // Also, because OrderBy is a stable sort, registration (dependency) order is kept.
                filteredProviders = filteredProviders.OrderBy(provider => contentTypesSelector(provider) != null && contentTypesSelector(provider).Count() != 0);
            }


            return filteredProviders;
        }
    }
}