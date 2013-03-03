using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.PathFinder;
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
        /// <param name="startNodeId"></param>
        /// <param name="targetNodeId"></param>
        /// <param name="settings"></param>
        PathResult FindPaths(IGraphContext graphContext, int startNodeId, int targetNodeId, IPathFinderSettings settings);
    }

    public interface IPathResult
    {
        IUndirectedGraph<int, IUndirectedEdge<int>> SucceededGraph { get; }
        IEnumerable<IEnumerable<int>> SucceededPaths { get; }
        IUndirectedGraph<int, IUndirectedEdge<int>> TraversedGraph { get; }
    }

    public class PathResult : IPathResult
    {
        public IUndirectedGraph<int, IUndirectedEdge<int>> SucceededGraph { get; protected set; }
        public IEnumerable<IEnumerable<int>> SucceededPaths { get; protected set; }
        public IUndirectedGraph<int, IUndirectedEdge<int>> TraversedGraph { get; protected set; }


        public PathResult(IUndirectedGraph<int, IUndirectedEdge<int>> succeededGraph, IEnumerable<IEnumerable<int>> succeededPaths, IUndirectedGraph<int, IUndirectedEdge<int>> traversedGraph)
        {
            SucceededGraph = succeededGraph;
            SucceededPaths = succeededPaths;
            TraversedGraph = traversedGraph;
        }
    }
}
