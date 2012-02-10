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
        GraphDescriptor FindGraph(IGraphContext graphContext);

        IEnumerable<GraphDescriptor> FindGraphs(IGraphContext graphContext);

        IEnumerable<GraphDescriptor> FindDistinctGraphs(IGraphContext graphContext);
    }
}
