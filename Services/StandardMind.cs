using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Associativy.Queryable;
using Orchard;
using QuickGraph;

namespace Associativy.Services
{
    public interface IStandardMind : IMind, IGraphAwareService, ITransientDependency
    {
    }

    public class StandardMind : GraphAwareServiceBase, IStandardMind
    {
        protected readonly IQueryableGraphFactory _queryableFactory;
        protected readonly IGraphEditor _graphEditor;
        protected readonly IMindEventHandler _eventHandler;
        protected readonly IGraphCacheService _cacheService;

        protected const string CachePrefix = "Associativy.StandardMind.";


        public StandardMind(
            IGraphDescriptor graphDescriptor,
            IQueryableGraphFactory queryableFactory,
            IGraphEditor graphEditor,
            IMindEventHandler eventHandler,
            IGraphCacheService cacheService)
            : base(graphDescriptor)
        {
            _queryableFactory = queryableFactory;
            _graphEditor = graphEditor;
            _eventHandler = eventHandler;
            _cacheService = cacheService;
        }


        public virtual IQueryableGraph<int> GetAllAssociations(IMindSettings settings)
        {
            MakeSettings(ref settings);

            var queryable = _queryableFactory.Create<int>(
                (parameters) =>
                {
                    var method = parameters.Method;
                    var zoom = parameters.Zoom;
                    var paging = parameters.Paging;
                    var graphInfo = _graphDescriptor.Services.ConnectionManager.GetGraphInfo();

                    if ((method == ExecutionMethod.NodeCount || method == ExecutionMethod.ConnectionCount)
                            && zoom.IsFlat() && parameters.Paging.SkipConnections == 0)
                    {
                        var totalConnectionCount = graphInfo.ConnectionCount;

                        if (method == ExecutionMethod.ConnectionCount)
                        {
                            return totalConnectionCount > parameters.Paging.TakeConnections ? parameters.Paging.TakeConnections : totalConnectionCount;
                        }

                        if (method == ExecutionMethod.NodeCount && totalConnectionCount > parameters.Paging.TakeConnections)
                        {
                            return graphInfo.NodeCount;
                        }
                    }


                    if (graphInfo.ConnectionCount > paging.SkipConnections + paging.TakeConnections)
                    {
                        var query =
                            _graphDescriptor.Services.PathFinder
                            .GetPartialGraph(graphInfo.CentralNodeId, new PathFinderSettings { MaxDistance = settings.MaxDistance });

                        return query.ExecuteWithParams(parameters);
                    }

                    var graph = _cacheService.GetMonitored(_graphDescriptor, QueryableGraphHelper.MakeCacheKey(MakeCacheKey("GetAllAssociations.BaseGraph", settings), parameters), () =>
                        {
                            var g = _graphEditor.GraphFactory<int>();

                            if (parameters.Zoom.Count == 0 || graphInfo.ConnectionCount <= paging.SkipConnections) return g;

                            // This won't include nodes that are not connected to anything
                            g.AddVerticesAndEdgeRange(
                                _graphDescriptor.Services.ConnectionManager.GetAll(paging.SkipConnections, paging.TakeConnections)
                                    .Select(connector => new UndirectedEdge<int>(connector.Node1Id, connector.Node2Id)));

                            return (IUndirectedGraph<int, IUndirectedEdge<int>>)g;
                        });


                    return LastSteps(parameters, graph, "GetAllAssociations", settings);
                });

            _eventHandler.AllAssociationsGraphBuilt(new AllAssociationsGraphBuiltContext(_graphDescriptor, settings, queryable));

            return queryable;
        }

        public virtual IQueryableGraph<int> MakeAssociations(IEnumerable<int> nodeIds, IMindSettings settings)
        {
            if (nodeIds == null) throw new ArgumentNullException("nodeIds");

            var nodeCount = nodeIds.Count();
            if (nodeCount == 0) throw new ArgumentException("The list of searched node ids can't be empty");

            MakeSettings(ref settings);

            IQueryableGraph<int> queryable;

            // If there's only one node, return its neighbours
            if (nodeCount == 1)
            {
                queryable = GetNeighboursGraph(nodeIds.First(), settings);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (settings.Algorithm == "simple")
            {
                queryable = MakeSimpleAssociations(nodeIds, settings);
            }
            // Calculate the routes between two nodes
            else
            {
                queryable = MakeSophisticatedAssociations(nodeIds, settings);
            }

            _eventHandler.SearchedGraphBuilt(new SearchedGraphBuiltContext(_graphDescriptor, settings, nodeIds, queryable));

            return queryable;
        }


        protected virtual IQueryableGraph<int> GetNeighboursGraph(int nodeId, IMindSettings settings)
        {
            return _queryableFactory.Create<int>(
                (parameters) =>
                {
                    var method = parameters.Method;
                    var zoom = parameters.Zoom;

                    var graph = _cacheService.GetMonitored(_graphDescriptor, QueryableGraphHelper.MakeCacheKey(MakeCacheKey("GetNeighboursGraph.BaseGrap." + nodeId, settings), parameters), () =>
                        {
                            var g = _graphEditor.GraphFactory<int>();

                            g.AddVertex(nodeId);
                            g.AddVerticesAndEdgeRange(
                                _graphDescriptor.Services.ConnectionManager.GetNeighbourIds(nodeId, parameters.Paging.SkipConnections, parameters.Paging.TakeConnections)
                                    .Select(neighbourId => new UndirectedEdge<int>(nodeId, neighbourId)));

                            return (IUndirectedGraph<int, IUndirectedEdge<int>>)g;
                        });

                    
                    return LastSteps(parameters, graph, "GetNeighboursGraph." + nodeId, settings);
                });
        }

        protected virtual IQueryableGraph<int> MakeSimpleAssociations(IEnumerable<int> nodeIds, IMindSettings settings)
        {
            return _queryableFactory.Create<int>(
                (parameters) =>
                {
                    var idsJoined = string.Join(", ", nodeIds.Select(node => node.ToString()));

                    var graph = _cacheService.GetMonitored(_graphDescriptor, QueryableGraphHelper.MakeCacheKey(MakeCacheKey("SimpleAssociations.BaseGrap." + idsJoined, settings), parameters), () =>
                        {
                            var paging = parameters.Paging;

                            // Simply calculate the intersection of the neighbours of the nodes
                            var g = _graphEditor.GraphFactory<int>();

                            var commonNeighbourIds = _graphDescriptor.Services.ConnectionManager.GetNeighbourIds(nodeIds.First());
                            var remainingNodes = new List<int>(nodeIds); // Maybe later we will need all the searched nodes
                            remainingNodes.RemoveAt(0);
                            commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(_graphDescriptor.Services.ConnectionManager.GetNeighbourIds(node)).ToList()).Skip(paging.SkipConnections).Take(paging.TakeConnections);
                            // Same as
                            //foreach (var node in remainingNodes)
                            //{
                            //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.ContentItem.Id)).ToList();
                            //}

                            foreach (var node in nodeIds)
                            {
                                foreach (var neighbourId in commonNeighbourIds)
                                {
                                    g.AddVerticesAndEdge(new UndirectedEdge<int>(node, neighbourId));
                                }
                            }

                            return (IUndirectedGraph<int, IUndirectedEdge<int>>)g;
                        });


                    return LastSteps(parameters, graph, "SimpleAssociations." + idsJoined, settings);
                });
        }

        protected virtual IQueryableGraph<int> MakeSophisticatedAssociations(IEnumerable<int> nodeIds, IMindSettings settings)
        {
            var nodeList = nodeIds.ToList();
            var nodeCount = nodeList.Count;
            if (nodeCount < 2) throw new ArgumentException("The count of nodes should be at least two.");

            Func<IEnumerable<IEnumerable<int>>, List<int>> getSucceededNodeIds =
                (currentSucceededPaths) =>
                {
                    var succeededNodeIds = new List<int>(currentSucceededPaths.Count()); // An incorrect estimate, but (micro)enhaces performance

                    foreach (var row in currentSucceededPaths)
                    {
                        succeededNodeIds = succeededNodeIds.Union(row).ToList();
                    }

                    return succeededNodeIds;
                };

            return _queryableFactory.Create<int>(
                (parameters) =>
                {
                    var idsJoined = string.Join(", ", nodeIds.Select(node => node.ToString()));

                    var graph = _cacheService.GetMonitored(_graphDescriptor, MakeCacheKey("SophisticatedAssociations.BaseGraph." + idsJoined, settings), () =>
                        {
                            var g = _graphEditor.GraphFactory<int>();
                            IList<IEnumerable<int>> succeededPaths;

                            Func<IUndirectedGraph<int, IUndirectedEdge<int>>> emptyResult = () =>  _graphEditor.GraphFactory<int>();

                            var pathFinderSettings = new PathFinderSettings { MaxDistance = settings.MaxDistance };
                            var allPairSucceededPaths = _graphDescriptor.Services.PathFinder.FindPaths(nodeList[0], nodeList[1], pathFinderSettings).SucceededPaths;

                            if (allPairSucceededPaths.Count() == 0) return emptyResult();

                            if (nodeList.Count == 2)
                            {
                                succeededPaths = allPairSucceededPaths.ToList();
                            }
                            // Calculate the routes between every nodes pair, then calculate the intersection of the routes
                            else
                            {
                                // We have to preserve the searched node ids in the succeeded paths despite the intersections
                                var searchedNodeIds = new List<int>(nodeList.Count);
                                nodeIds.ToList().ForEach(
                                        node => searchedNodeIds.Add(node)
                                    );

                                var commonSucceededNodeIds = getSucceededNodeIds(allPairSucceededPaths).Union(searchedNodeIds).ToList();

                                for (int i = 0; i < nodeList.Count - 1; i++)
                                {
                                    int n = i + 1;
                                    if (i == 0) n = 2; // Because of the calculation of intersections the first iteration is already done above

                                    while (n < nodeList.Count)
                                    {
                                        // Here could be multithreading
                                        var pairSucceededPaths = _graphDescriptor.Services.PathFinder.FindPaths(nodeList[i], nodeList[n], pathFinderSettings).SucceededPaths;
                                        commonSucceededNodeIds = commonSucceededNodeIds.Intersect(getSucceededNodeIds(pairSucceededPaths).Union(searchedNodeIds)).ToList();
                                        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();

                                        n++;
                                    }
                                }


                                if (allPairSucceededPaths.Count() == 0 || commonSucceededNodeIds.Count == 0) return emptyResult();

                                succeededPaths = new List<IEnumerable<int>>(allPairSucceededPaths.Count()); // We are oversizing, but it's worth the performance gain

                                foreach (var path in allPairSucceededPaths)
                                {
                                    var succeededPath = path.Intersect(commonSucceededNodeIds);
                                    if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath); // Only paths where intersecting nodes are present
                                }


                                if (succeededPaths.Count() == 0) return emptyResult();
                            }

                            g.AddVertexRange(getSucceededNodeIds(succeededPaths));

                            foreach (var path in succeededPaths)
                            {
                                var pathList = path.ToList();
                                for (int i = 1; i < pathList.Count; i++)
                                {
                                    var node1Id = pathList[i - 1];
                                    var node2Id = pathList[i];

                                    // Despite the graph being undirected and not allowing parallel edges, the same edges, registered with a 
                                    // different order of source and dest are recognized as different edges.
                                    // See issue: http://quickgraph.codeplex.com/workitem/21805
                                    var newEdge = new UndirectedEdge<int>(node1Id, node2Id);
                                    IUndirectedEdge<int> reversedNewEdge;

                                    // It's sufficient to only check the reversed edge; if newEdge is present it will be overwritten without problems
                                    if (!g.TryGetEdge(node2Id, node1Id, out reversedNewEdge))
                                    {
                                        g.AddEdge(newEdge);
                                    }
                                }
                            }

                            return g;
                        });

                    return QueryableGraphHelper.LastStepsWithPaging(new LastStepParams
                    {
                        CacheService = _cacheService,
                        GraphEditor = _graphEditor,
                        GraphDescriptor = _graphDescriptor,
                        ExecutionParameters = parameters,
                        Graph = graph,
                        BaseCacheKey = MakeCacheKey("SophisticatedAssociations." + idsJoined, settings)
                    });
                });
        }

        protected dynamic LastSteps(IExecutionParams parameters, IUndirectedGraph<int, IUndirectedEdge<int>> graph, string cacheName, IMindSettings settings)
        {
            return QueryableGraphHelper.LastSteps(new LastStepParams
            {
                CacheService = _cacheService,
                GraphEditor = _graphEditor,
                GraphDescriptor = _graphDescriptor,
                ExecutionParameters = parameters,
                Graph = graph,
                BaseCacheKey = MakeCacheKey(cacheName, settings)
            });
        }

        protected string MakeCacheKey(string name, IMindSettings settings)
        {
            return CachePrefix + _graphDescriptor.Name + "." + name + ".MindSettings:" + settings.Algorithm + "/" + settings.MaxDistance;
        }

        protected void MakeSettings(ref IMindSettings settings)
        {
            if (settings == null) settings = MindSettings.Default;
        }
    }
}