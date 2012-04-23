using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    public interface IGraphDescriptorFilterer : IDependency
    {
        IEnumerable<GraphDescriptor> FilterByMatchingGraphContext(IEnumerable<GraphDescriptor> descriptors, IGraphContext graphContext);
    }
}
