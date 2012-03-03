using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    public interface IDescriptorFilterer : IDependency
    {
        IEnumerable<TAssociativyDescriptor> FilterByMatchingGraphContext<TAssociativyDescriptor>(
            IEnumerable<TAssociativyDescriptor> descriptors,
            IGraphContext graphContext,
            Func<TAssociativyDescriptor, string> graphNameSelector,
            Func<TAssociativyDescriptor, IEnumerable<string>> contentTypesSelector);
    }

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
