using System.Collections.Generic;
using Orchard;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Handles registered graphs
    /// </summary>
    public interface IGraphManager : IDependency
    {
        /// <summary>
        /// Gets the last graph from the registered graphs that best matches the specified context
        /// </summary>
        /// <param name="graphContext">The graph context instance to match against</param>
        GraphDescriptor FindGraph(IGraphContext graphContext);

        /// <summary>
        /// Lists all registered graphs that match the specified context
        /// </summary>
        /// <param name="graphContext">The graph context instance to match against</param>
        /// <returns></returns>
        IEnumerable<GraphDescriptor> FindGraphs(IGraphContext graphContext);

        /// <summary>
        /// Lists graphs that match the specified context from the registered graphs and makes them distinct by looking at the graph name
        /// </summary>
        /// <param name="graphContext">The graph context instance to match against</param>
        IEnumerable<GraphDescriptor> FindDistinctGraphs(IGraphContext graphContext);
    }
}
