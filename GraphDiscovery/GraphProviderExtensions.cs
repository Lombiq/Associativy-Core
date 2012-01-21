using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.GraphDiscovery
{
    public static class GraphProviderExtensions
    {
        /// <summary>
        /// Creates the maximal context the provider supports
        /// </summary>
        public static IGraphContext ProduceContext(this IGraphProvider provider)
        {
            return new GraphContext { GraphName = provider.GraphName, ContentTypes = provider.ContentTypes };
        }
    }
}