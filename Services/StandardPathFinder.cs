using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Orchard;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using QuickGraph;
using System;
using Associativy.Queryable;
using Orchard.ContentManagement;

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
        protected readonly IQueryableGraphFactory _queryableFactory;


        public StandardPathFinder(
            IGraphDescriptor graphDescriptor,
            IGraphEditor graphEditor,
            IGraphEventMonitor graphEventMonitor,
            ICacheManager cacheManager,
            IQueryableGraphFactory queryableFactory)
            : base(graphDescriptor)
        {
            _graphEditor = graphEditor;
            _graphEventMonitor = graphEventMonitor;
            _cacheManager = cacheManager;
            _queryableFactory = queryableFactory;
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


        public virtual IPathResult FindPaths(int startNodeId, int targetNodeId, IPathFinderSettings settings)
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
                    return FindPathsUncached(startNodeId, targetNodeId, settings);
                });
            }

            return FindPathsUncached(startNodeId, targetNodeId, settings);
        }

        public virtual IQueryableGraph<int> GetPartialGraph(int centralNodeId, IPathFinderSettings settings)
        {
            if (settings == null) settings = PathFinderSettings.Default;

            return _queryableFactory.Factory<int>((parameters) =>
                {
                    // TODO: caching
                    var connectionManager = _graphDescriptor.Services.ConnectionManager;
                    var graph = _graphEditor.GraphFactory<int>();
                    var visited = new Dictionary<int, PathNode>();
                    var frontier = new Stack<PathNode>();

                    frontier.Push(new PathNode(centralNodeId) { MinDistance = 0 });

                    while (frontier.Count != 0)
                    {
                        var current = frontier.Pop();

                        if (current.MinDistance == settings.MaxDistance) continue;

                        if (!visited.ContainsKey(current.Id))
                        {
                            visited[current.Id] = current;

                            foreach (var neighbourId in connectionManager.GetNeighbourIds(current.Id))
                            {
                                var neighbour = visited.ContainsKey(neighbourId) ? visited[neighbourId] : new PathNode(neighbourId);

                                current.Neighbours.Add(neighbour);
                                graph.AddVerticesAndEdge(new UndirectedEdge<int>(current.Id, neighbourId));
                            } 
                        }


                        var neighbourMinDistance = current.MinDistance + 1;
                        foreach (var neighbour in current.Neighbours)
                        {
                            if (neighbourMinDistance < neighbour.MinDistance) neighbour.MinDistance = neighbourMinDistance;
                            frontier.Push(neighbour);
                        }
                    }

                    return graph;
                });
        }

        protected IPathResult FindPathsUncached(int startNodeId, int targetNodeId, IPathFinderSettings settings)
        {
            // This below is a depth-first search that tries to find all paths to the target node that are within the maximal length (maxDistance) and
            // keeps track of the paths found.
            var connectionManager = _graphDescriptor.Services.ConnectionManager;

            var explored = new Dictionary<int, PathNode>();
            var succeededPaths = new List<List<int>>();
            var frontier = new Stack<FrontierNode>();

            explored[startNodeId] = new PathNode(startNodeId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startNodeId] });

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
                        }

                        if (explored[targetNodeId].MinDistance > currentDistance + 1)
                        {
                            explored[targetNodeId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetNodeId]);
                        currentPath.Add(targetNodeId);
                        succeededPaths.Add(currentPath);
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
                            succeededPaths.Add(succeededPath);
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
                        }
                    }
                }
            }

            return new PathResult
                {
                    SucceededPaths = succeededPaths,
                    SucceededGraph = PathToGraph(succeededPaths)
                };
        }


        private IQueryableGraph<int> PathToGraph(List<List<int>> succeededPaths)
        {
            return _queryableFactory.Factory<int>((parameters) =>
                {
                    var method = parameters.Method;
                    var zoom = parameters.Zoom;
                    var paging = parameters.Paging;

                    var graph = _graphEditor.GraphFactory<int>();

                    Action fillGraph =
                        () =>
                        {
                            if (paging.TakeConnections == 0) return;

                            var skipped = 0;
                            var enumerated = 0;
                            foreach (var path in succeededPaths)
                            {
                                var iStart = 1;

                                if (skipped < paging.SkipConnections)
                                {
                                    if (path.Count < paging.SkipConnections - skipped)
                                    {
                                        skipped += path.Count;
                                        iStart = path.Count;
                                    }
                                    else
                                    {
                                        iStart += paging.SkipConnections - skipped;
                                        skipped = paging.SkipConnections;
                                    }
                                }

                                for (int i = iStart; i < path.Count; i++)
                                {
                                    graph.AddVerticesAndEdge(new UndirectedEdge<int>(path[i - 1], path[i]));
                                    enumerated++;
                                    if (enumerated == paging.TakeConnections) return;
                                }
                            }
                        };

                    switch (parameters.Method)
                    {
                        case ExecutionMethod.NodeCount:
                            return graph.VertexCount;
                        case ExecutionMethod.ConnectionCount:
                            return graph.EdgeCount;
                        case ExecutionMethod.ZoomLevelCount:
                            return _graphEditor.CalculateZoomLevelCount(graph, parameters.Zoom.Count);
                        default:
                            if (!zoom.IsFlat()) return _graphEditor.CreateZoomedGraph(graph, zoom.Level, zoom.Count);
                            return graph;
                    }
                });
        }


        private class PathResult : IPathResult
        {
            public IQueryableGraph<int> SucceededGraph { get; set; }
            public IEnumerable<IEnumerable<int>> SucceededPaths { get; set; }
        }
    }
}