using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

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
}
