using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.GraphDiscovery
{
    public static class GraphDescriptorExtensions
    {
        /// <summary>
        /// Creates the maximal context the descriptor supports
        /// </summary>
        public static IGraphContext ProduceMaximalContext(this GraphDescriptor descriptor)
        {
            return new GraphContext { GraphName = descriptor.GraphName, ContentTypes = descriptor.ContentTypes };
        }
    }
}