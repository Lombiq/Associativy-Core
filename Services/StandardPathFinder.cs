using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Orchard;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations, by default using IConnectionManager
    /// </summary>
    /// <remarks>
    /// E.g. when using a graph database as storage, you can write your own IPathFinder and IConnectionManager implementation for optimized results.
    /// </remarks>
    public interface IStandardPathFinder : IPathFinder, IDependency
    {
    }

    [OrchardFeature("Associativy")]
    public class StandardPathFinder : GraphServiceBase, IStandardPathFinder
    {
        protected readonly IGraphEditor _graphEditor;
        protected readonly IGraphEventMonitor _graphEventMonitor;
        protected readonly ICacheManager _cacheManager;


        public StandardPathFinder(
            IGraphDescriptor graphDescriptor,
            IGraphEditor graphEditor,
            IGraphEventMonitor graphEventMonitor,
            ICacheManager cacheManager)
            : base(graphDescriptor)
        {
            _graphEditor = graphEditor;
            _graphEventMonitor = graphEventMonitor;
            _cacheManager = cacheManager;
        }


        #region FindPaths() auxiliary classes
        protected class PathNode
        {
            public int Id { get; private set; }
            public int MinDistance { get; set; }
            public List<PathNode> Neighbours { get; set; }

            public PathNode(int id)
            {
                Id = id;
                MinDistance = int.MaxValue;
                Neighbours = new List<PathNode>();
            }
        }

        protected class FrontierNode
        {
            public int Distance { get; set; }
            public List<int> Path { get; set; }
            public PathNode Node { get; set; }

            public FrontierNode()
            {
                Distance = 0;
                Path = new List<int>();
            }
        }
        #endregion

        public virtual PathResult FindPaths(int startNodeId, int targetNodeId, IPathFinderSettings settings)
        {
            if (settings == null) settings = PathFinderSettings.Default;

            // It could be that this is the only caching that's really needed and can work:
            // - With this tens of database queries can be saved (when the connection manager uses DB storage without in-memory caching).
            // - Caching the whole graph would be nice, but caching parts and their records cause problems.
            if (settings.UseCache)
            {
                return _cacheManager.Get("Associativy.Paths." + _graphDescriptor.Name + startNodeId.ToString() + targetNodeId.ToString() + settings.MaxDistance, ctx =>
                {
                    _graphEventMonitor.MonitorChanged(_graphDescriptor, ctx);
                    return FindPaths(startNodeId, targetNodeId, settings);
                });
            }

            // This below is a depth-first search that tries to find all paths to the target node that are within the maximal length (maxDistance) and
            // keeps track of the paths found.
            var connectionManager = _graphDescriptor.Services.ConnectionManager;

            var explored = new Dictionary<int, PathNode>();
            var succeededGraph = _graphEditor.GraphFactory<int>();
            var succeededPaths = new List<List<int>>();
            var traversedGraph = _graphEditor.GraphFactory<int>();
            var frontier = new Stack<FrontierNode>();

            explored[startNodeId] = new PathNode(startNodeId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startNodeId] });
            traversedGraph.AddVertex(startNodeId);

            FrontierNode frontierNode;
            PathNode currentNode;
            List<int> currentPath;
            int currentDistance;
            while (frontier.Count != 0)
            {
                frontierNode = frontier.Pop();
                currentNode = frontierNode.Node;
                currentPath = frontierNode.Path;
                currentPath.Add(currentNode.Id);
                currentDistance = frontierNode.Distance;

                // We can't traverse the graph further
                if (currentDistance == settings.MaxDistance - 1)
                {
                    // Target will be only found if it's the direct neighbour of current
                    if (connectionManager.AreNeighbours(currentNode.Id, targetNodeId))
                    {
                        if (!explored.ContainsKey(targetNodeId))
                        {
                            explored[targetNodeId] = new PathNode(targetNodeId);
                            traversedGraph.AddVertex(targetNodeId);
                        }

                        if (explored[targetNodeId].MinDistance > currentDistance + 1)
                        {
                            explored[targetNodeId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetNodeId]);
                        currentPath.Add(targetNodeId);
                        SavePathToGraph(succeededGraph, succeededPaths, currentPath);
                        traversedGraph.AddEdge(new UndirectedEdge<int>(currentNode.Id, targetNodeId));
                    }
                }
                // We can traverse the graph further
                else
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        var neighbourIds = connectionManager.GetNeighbourIds(currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count());
                        foreach (var neighbourId in neighbourIds)
                        {
                            currentNode.Neighbours.Add(new PathNode(neighbourId));
                        }
                    }

                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        // Target is a neighbour
                        if (neighbour.Id == targetNodeId)
                        {
                            var succeededPath = new List<int>(currentPath) { targetNodeId }; // Since we will use currentPath in further iterations too
                            SavePathToGraph(succeededGraph, succeededPaths, succeededPath);
                        }
                        // We can traverse further, push the neighbour onto the stack
                        else if (neighbour.Id != startNodeId)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                            if (!explored.ContainsKey(neighbour.Id) || neighbour.MinDistance >= currentDistance + 1)
                            {
                                frontier.Push(new FrontierNode { Distance = currentDistance + 1, Path = new List<int>(currentPath), Node = neighbour });
                            }
                        }

                        if (neighbour.Id != startNodeId)
                        {
                            // If this is the shortest path to the node, overwrite its minDepth
                            if (neighbour.MinDistance > currentDistance + 1)
                            {
                                neighbour.MinDistance = currentDistance + 1;
                            }

                            explored[neighbour.Id] = neighbour;
                            traversedGraph.AddVertex(neighbour.Id);
                            traversedGraph.AddEdge(new UndirectedEdge<int>(currentNode.Id, neighbour.Id));
                        }
                    }
                }
            }

            return new PathResult(succeededGraph, succeededPaths, traversedGraph);
        }


        private static void SavePathToGraph(IMutableUndirectedGraph<int, IUndirectedEdge<int>> succeededGraph, List<List<int>> succeededPaths, List<int> path)
        {
            for (int i = 1; i < path.Count; i++)
            {
                succeededGraph.AddVerticesAndEdge(new UndirectedEdge<int>(path[i-1], path[i]));
            }

            succeededPaths.Add(path);
        }
    }
}