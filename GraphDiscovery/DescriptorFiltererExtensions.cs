using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public static class DescriptorFiltererExtensions
    {
        public static IEnumerable<GraphDescriptor> FilterByMatchingGraphContext(
            this IDescriptorFilterer descriptorFilterer,
            IEnumerable<GraphDescriptor> descriptors, 
            IGraphContext graphContext)
        {
            return descriptorFilterer.FilterByMatchingGraphContext(descriptors, graphContext, (descriptor) => descriptor.GraphName, (descriptor) => descriptor.ContentTypes);
        }
    }
}