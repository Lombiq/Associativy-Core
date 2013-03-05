using System.Collections.Generic;
using Orchard;

namespace Associativy.GraphDiscovery
{
    public interface IGraphDescriptorFilterer : IDependency
    {
        IEnumerable<IGraphDescriptor> FilterByMatchingGraphContext(IEnumerable<IGraphDescriptor> descriptors, IGraphContext graphContext);
    }
}
