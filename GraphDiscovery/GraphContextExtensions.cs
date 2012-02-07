using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.GraphDiscovery
{
    public static class GraphContextExtensions
    {
        public static string Stringify(this IGraphContext graphContext)
        {
            return graphContext.GraphName + String.Join(" ", graphContext.ContentTypes);
        }
    }
}