using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Handles registered graphs
    /// </summary>
    public interface IGraphManager : IDependency
    {
        GraphDescriptor FindLastDescriptor(IGraphContext graphContext);

        IEnumerable<GraphDescriptor> FindDescriptors(IGraphContext graphContext);
    }
}
