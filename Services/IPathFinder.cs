using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;
using Associativy.GraphDiscovery;
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
        public IEnumerable<IEnumerable<int>> SucceededPaths { get; protected set; }
        public IUndirectedGraph<int, IUndirectedEdge<int>> TraversedGraph { get; protected set; }

        public PathResult()
        {
            SucceededPaths = new List<IEnumerable<int>>();
            TraversedGraph = new UndirectedGraph<int, IUndirectedEdge<int>>(false);
        }

        public PathResult(IEnumerable<IEnumerable<int>> succeededPaths, IUndirectedGraph<int, IUndirectedEdge<int>> traversedGraph)
        {
            SucceededPaths = succeededPaths;
            TraversedGraph = traversedGraph;
        }
    }
}
