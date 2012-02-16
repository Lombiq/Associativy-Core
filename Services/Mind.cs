using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using QuickGraph;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class Mind : AssociativyServiceBase, IMind
    {
        protected readonly INodeManager _nodeManager;
        protected readonly IPathFinder _pathFinder;
        protected readonly IGraphService _graphService;
        protected readonly IGraphEventMonitor _associativeGraphEventMonitor;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected string _cachePrefix;
        #endregion

        public Mind(
            IGraphManager graphManager,
            INodeManager nodeManager,
            IPathFinder pathFinder,
            IGraphService graphService,
            IGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
            : base(graphManager)
        {
            _nodeManager = nodeManager;
            _pathFinder = pathFinder;
            _graphService = graphService;
            _associativeGraphEventMonitor = associativeGraphEventMonitor;
            _cacheManager = cacheManager;
        }


        public virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(
            IGraphContext graphContext,
            IMindSettings settings = null)
        {
            var descriptor = _graphManager.FindGraph(graphContext);
            MakeSettings(ref settings);

            Func<IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>>> makeWholeGraph =
                () =>
                {
                    var wholeGraph = _graphService.GraphFactory();

                    var query =_nodeManager.GetContentQuery(graphContext);
                    settings.QueryModifier(query);
                    var nodes = query.List().ToDictionary<IContent, int>(node => node.Id);

                    foreach (var node in nodes)
                    {
                        wholeGraph.AddVertex(node.Value);
                    }

                    var connections = descriptor.ConnectionManager.GetAll(graphContext);
                    foreach (var connection in connections)
                    {
                        // Since the QueryModifier could have removed items, this check is necessary
                        if (nodes.ContainsKey(connection.Node1Id) && nodes.ContainsKey(connection.Node2Id))
                        {
                            wholeGraph.AddEdge(new UndirectedEdge<IContent>(nodes[connection.Node1Id], nodes[connection.Node2Id]));
                        }
                    }

                    return wholeGraph;
                };


            if (settings.UseCache)
            {
                var graph = _cacheManager.Get(MakeCacheKey("WholeGraph"), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(graphContext, ctx);
                    return makeWholeGraph();
                });

                return _cacheManager.Get(MakeCacheKey("WholeGraphZoomed.Zoom:" + settings.ZoomLevel, settings), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(graphContext, ctx);
                    return _graphService.CreateZoomedGraph(graph, settings.ZoomLevel, settings.MaxZoomLevel);
                });
            }
            else return _graphService.CreateZoomedGraph(makeWholeGraph(), settings.ZoomLevel, settings.MaxZoomLevel);
        }

        public virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(
            IGraphContext graphContext,
            IEnumerable<IContent> nodes,
            IMindSettings settings = null)
        {
            if (nodes == null) throw new ArgumentNullException("The list of searched nodes can't be empty");

            var nodeCount = nodes.Count();
            if (nodeCount == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            var descriptor = _graphManager.FindGraph(graphContext);
            MakeSettings(ref settings);

            Func<IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>>> makeGraph =
                () =>
                {
                    // If there's only one node, return its neighbours
                    if (nodeCount == 1)
                    {
                        return GetNeighboursGraph(graphContext, descriptor, nodes.First(), settings.QueryModifier);
                    }
                    // Simply calculate the intersection of the neighbours of the nodes
                    else if (settings.Algorithm == MindAlgorithm.Simple)
                    {
                        return MakeSimpleAssociations(graphContext, descriptor, nodes, settings.QueryModifier);
                    }
                    // Calculate the routes between two nodes
                    else
                    {
                        return MakeSophisticatedAssociations(graphContext, descriptor, nodes, settings);
                    }
                };

            if (settings.UseCache)
            {
                string cacheKey = "AssociativeGraph.";
                nodes.ToList().ForEach(node => cacheKey += node.Id.ToString() + ", ");

                var graph = _cacheManager.Get(MakeCacheKey(cacheKey, settings), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(graphContext, ctx);
                    return makeGraph();
                });

                return _cacheManager.Get(MakeCacheKey(cacheKey + ".Zoom" + settings.ZoomLevel, settings), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(graphContext, ctx);
                    return _graphService.CreateZoomedGraph(graph, settings.ZoomLevel, settings.MaxZoomLevel);
                });
            }
            else return _graphService.CreateZoomedGraph(makeGraph(), settings.ZoomLevel, settings.MaxZoomLevel);
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetNeighboursGraph(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IContent node,
            Action<IContentQuery<ContentItem>> queryModifier)
        {
            var graph = _graphService.GraphFactory();

            graph.AddVertex(node);

            var query = _nodeManager.GetManyContentQuery(graphContext, descriptor.ConnectionManager.GetNeighbourIds(graphContext, node.Id));
            queryModifier(query);
            var neighbours = query.List();
            
            foreach (var neighbour in neighbours)
            {
                graph.AddVerticesAndEdge(new UndirectedEdge<IContent>(node, neighbour));
            }

            return graph;
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeSimpleAssociations(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IEnumerable<IContent> nodes,
            Action<IContentQuery<ContentItem>> queryModifier)
        {
            // Simply calculate the intersection of the neighbours of the nodes

            var graph = _graphService.GraphFactory();

            var commonNeighbourIds = descriptor.ConnectionManager.GetNeighbourIds(graphContext, nodes.First().Id);
            var remainingNodes = new List<IContent>(nodes); // Maybe later we will need all the searched nodes
            remainingNodes.RemoveAt(0);
            commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(descriptor.ConnectionManager.GetNeighbourIds(graphContext, node.Id)).ToList());
            // Same as
            //foreach (var node in remainingNodes)
            //{
            //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList();
            //}

            if (commonNeighbourIds.Count() == 0) return graph;

            var query = _nodeManager.GetManyContentQuery(graphContext, commonNeighbourIds);
            queryModifier(query);
            var commonNeighbours = query.List();

            foreach (var node in nodes)
            {
                foreach (var neighbour in commonNeighbours)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<IContent>(node, neighbour));
                }
            }

            return graph;
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeSophisticatedAssociations(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IEnumerable<IContent> nodes,
            IMindSettings settings)
        {
            var nodeList = nodes.ToList();
            if (nodeList.Count < 2) throw new ArgumentException("The count of nodes should be at least two.");

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


            var graph = _graphService.GraphFactory();
            IList<IEnumerable<int>> succeededPaths;

            var allPairSucceededPaths = _pathFinder.FindPaths(graphContext, nodeList[0].Id, nodeList[1].Id, settings.MaxDistance, settings.UseCache);

            if (allPairSucceededPaths.Count() == 0) return graph;

            if (nodeList.Count == 2)
            {
                succeededPaths = allPairSucceededPaths.ToList();
            }
            // Calculate the routes between every nodes pair, then calculate the intersection of the routes
            else
            {
                // We have to preserve the searched node ids in the succeeded paths despite the intersections
                var searchedNodeIds = new List<int>(nodeList.Count);
                nodes.ToList().ForEach(
                        node => searchedNodeIds.Add(node.Id)
                    );

                var commonSucceededNodeIds = getSucceededNodeIds(allPairSucceededPaths).Union(searchedNodeIds).ToList();

                for (int i = 0; i < nodeList.Count - 1; i++)
                {
                    int n = i + 1;
                    if (i == 0) n = 2; // Because of the calculation of intersections the first iteration is already done above

                    while (n < nodeList.Count)
                    {
                        // Here could be multithreading
                        var pairSucceededPaths = _pathFinder.FindPaths(graphContext, nodeList[i].Id, nodeList[n].Id, settings.MaxDistance, settings.UseCache);
                        commonSucceededNodeIds = commonSucceededNodeIds.Intersect(getSucceededNodeIds(pairSucceededPaths).Union(searchedNodeIds)).ToList();
                        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();

                        n++;
                    }
                }

                if (allPairSucceededPaths.Count() == 0 || commonSucceededNodeIds.Count == 0) return graph;

                succeededPaths = new List<IEnumerable<int>>(allPairSucceededPaths.Count()); // We are oversizing, but it's worth the performance gain

                foreach (var path in allPairSucceededPaths)
                {
                    var succeededPath = path.Intersect(commonSucceededNodeIds);
                    if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath); // Only paths where intersecting nodes are present
                }

                if (succeededPaths.Count() == 0) return graph;
            }

            var query = _nodeManager.GetManyContentQuery(graphContext, getSucceededNodeIds((succeededPaths)));
            settings.QueryModifier(query);
            var succeededNodes = query.List().ToDictionary(node => node.Id);

            graph.AddVertexRange(succeededNodes.Values);

            foreach (var path in succeededPaths)
            {
                var pathList = path.ToList();
                for (int i = 1; i < pathList.Count; i++)
                {
                    var node1Id = pathList[i - 1];
                    var node2Id = pathList[i];

                    // Since the QueryModifier could have removed items, this check is necessary
                    if (succeededNodes.ContainsKey(node1Id) && succeededNodes.ContainsKey(node2Id))
                    {
                        // Despite the graph being undirected and not allowing parallel edges, the same edges, registered with a 
                        // different order of source and dest are recognized as different edges.
                        // See issue: http://quickgraph.codeplex.com/workitem/21805
                        var newEdge = new UndirectedEdge<IContent>(succeededNodes[node1Id], succeededNodes[node2Id]);
                        IUndirectedEdge<IContent> reversedNewEdge;

                        // It's sufficient to only check the reversed edge; if newEdge is present it will be overwritten without problems
                        if (!graph.TryGetEdge(succeededNodes[node2Id], succeededNodes[node1Id], out reversedNewEdge))
                        {
                            graph.AddEdge(newEdge);
                        }
                    }
                }
            }

            return graph;
        }

        protected string MakeCacheKey(string name, IMindSettings settings)
        {
            return MakeCacheKey(name)
                + "MindSettings:" + settings.Algorithm + settings.MaxDistance;
        }

        protected string MakeCacheKey(string name)
        {
            return _cachePrefix + name;
        }

        protected void MakeSettings(ref IMindSettings settings)
        {
            if (settings == null) settings = new MindSettings();
        }
    }
}