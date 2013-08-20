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
        IGraphDescriptor FindGraph(IGraphContext graphContext);

        /// <summary>
        /// Lists all registered graphs that match the specified context
        /// </summary>
        /// <param name="graphContext">The graph context instance to match against</param>
        IEnumerable<IGraphDescriptor> FindGraphs(IGraphContext graphContext);
    }


    public static class GraphManagerExtensions
    {
        public static IGraphDescriptor FindGraphByName(this IGraphManager graphManager, string name)
        {
            return graphManager.FindGraph(new GraphContext { Name = name });
        }
    }
}
