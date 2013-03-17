using System.Collections.Generic;
using System.Linq;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Associativy.Queryable;
using Orchard;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations, by default using IConnectionManager
    /// </summary>
    /// <remarks>
    /// E.g. when using a graph database as storage, you can write your own IPathFinder and IConnectionManager implementation for optimized results.
    /// </remarks>
    public interface IStandardPathFinder : IPathFinder, IGraphAwareService, ITransientDependency
    {
    }

    public class StandardPathFinder : GraphAwareServiceBase, IStandardPathFinder
    {
        protected readonly IPathFinderAuxiliaries _pathFinderAuxiliaries;

        protected const string CachePrefix = "Associativy.StandardPathFinder.";


        public StandardPathFinder(
            IGraphDescriptor graphDescriptor,
            IPathFinderAuxiliaries pathFinderAuxiliaries)
            : base(graphDescriptor)
        {
            _pathFinderAuxiliaries = pathFinderAuxiliaries;
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

            var paths = _pathFinderAuxiliaries.CacheService.GetMonitored(_graphDescriptor, MakeCacheKey("FindPaths.Paths." + _graphDescriptor.Name + "/" + startNodeId.ToString() + "/" + targetNodeId.ToString(), settings), () =>
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

                    return succeededPaths;
                });


            return new Associativy.Services.PathFinderAuxiliaries.PathResult
            {
                SucceededPaths = paths,
                SucceededGraph = PathToGraph(paths, "PathToGraph:" + startNodeId + "/" + targetNodeId, settings)
            };
        }

        public virtual IQueryableGraph<int> GetPartialGraph(int centralNodeId, IPathFinderSettings settings)
        {
            if (settings == null) settings = PathFinderSettings.Default;

            return _pathFinderAuxiliaries.QueryableFactory.Create<int>((parameters) =>
                {
                    var graph = _pathFinderAuxiliaries.CacheService.GetMonitored(_graphDescriptor, MakeCacheKey("GetPartialGraph.BaseGraph." + centralNodeId, settings), () =>
                    {
                        var connectionManager = _graphDescriptor.Services.ConnectionManager;
                        var g = _pathFinderAuxiliaries.GraphEditor  .GraphFactory<int>();
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
                                    g.AddVerticesAndEdge(new UndirectedEdge<int>(current.Id, neighbourId));
                                }
                            }


                            var neighbourMinDistance = current.MinDistance + 1;
                            foreach (var neighbour in current.Neighbours)
                            {
                                if (neighbourMinDistance < neighbour.MinDistance)
                                {
                                    neighbour.MinDistance = neighbourMinDistance;
                                    frontier.Push(neighbour);
                                }
                            }
                        }

                        return g;
                    });


                    return LastStepsWithPaging(parameters, graph, "GetPartialGraph." + centralNodeId + ".PathToGraph.", settings);
                });
        }


        private IQueryableGraph<int> PathToGraph(IEnumerable<IList<int>> succeededPaths, string baseCacheKey, IPathFinderSettings settings)
        {
            return _pathFinderAuxiliaries.QueryableFactory.Create<int>((parameters) =>
                {
                    var graph = _pathFinderAuxiliaries.CacheService.GetMonitored(_graphDescriptor, MakeCacheKey(baseCacheKey + "BaseGraph.", settings), () =>
                    {
                        return _pathFinderAuxiliaries.PathToGraph(succeededPaths);
                    });


                    return LastStepsWithPaging(parameters, graph, baseCacheKey, settings);
                });
        }

        private dynamic LastStepsWithPaging(IExecutionParams parameters, IUndirectedGraph<int, IUndirectedEdge<int>> graph, string cacheName, IPathFinderSettings settings)
        {
            return QueryableGraphHelper.LastStepsWithPaging(new LastStepParams
            {
                CacheService = _pathFinderAuxiliaries.CacheService,
                GraphEditor = _pathFinderAuxiliaries.GraphEditor,
                GraphDescriptor = _graphDescriptor,
                ExecutionParameters = parameters,
                Graph = graph,
                BaseCacheKey = MakeCacheKey(cacheName, settings)
            });
        }

        private string MakeCacheKey(string name, IPathFinderSettings settings)
        {
            return CachePrefix + _graphDescriptor.Name + "." + name + ".PathFinderSettings:" + settings.MaxDistance;
        }
    }
}