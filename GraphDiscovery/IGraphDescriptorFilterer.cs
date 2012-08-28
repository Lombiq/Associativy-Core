using System.Collections.Generic;
using Orchard;

namespace Associativy.GraphDiscovery
{
    public interface IGraphDescriptorFilterer : IDependency
    {
        IEnumerable<GraphDescriptor> FilterByMatchingGraphContext(IEnumerable<GraphDescriptor> descriptors, IGraphContext graphContext);
    }
}
