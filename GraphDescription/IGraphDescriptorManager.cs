using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.GraphDescription
{
    /// <summary>
    /// Handles registered graphDescriptors
    /// </summary>
    public interface IGraphDescriptorManager : IDependency
    {
        IGraphDescriptor FindDescriptor(IGraphContext graphContext);

        bool TryFindDescriptorsForContentType(IContentContext contentContext, out IEnumerable<IGraphDescriptor> graphDescriptors);
    }
}
