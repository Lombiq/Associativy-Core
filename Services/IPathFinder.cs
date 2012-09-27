using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Orchard;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations
    /// </summary>
    public interface IPathFinder : IDependency
    {
        /// <summary>
        /// Calculates all paths between two nodes, depending on the settings.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <param name="maxDistance"></param>
        /// <param name="useCache"></param>
        PathResult FindPaths(IGraphContext graphContext, int startNodeId, int targetNodeId, int maxDistance = 3, bool useCache = false);
    }

    public class PathResult
    {
        public IUndirectedGraph<int, IUndirectedEdge<int>> SucceededGraph { get; protected set; }
        public IUndirectedGraph<int, IUndirectedEdge<int>> TraversedGraph { get; protected set; }

        public PathResult(IUndirectedGraph<int, IUndirectedEdge<int>> succeededGraph, IUndirectedGraph<int, IUndirectedEdge<int>> traversedGraph)
        {
            SucceededGraph = succeededGraph;
            TraversedGraph = traversedGraph;
        }
    }
}
