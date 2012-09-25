using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Mind;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using QuickGraph;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class Mind : AssociativyServiceBase, IMind
    {
        protected readonly INodeManager _nodeManager;
        protected readonly IGraphEditor _graphEditor;
        protected readonly IGraphEventMonitor _graphEventMonitor;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected const string _cachePrefix = "Associativy.";
        #endregion

        public Mind(
            IGraphManager graphManager,
            INodeManager nodeManager,
            IGraphEditor graphEditor,
            IGraphEventMonitor graphEventMonitor,
            ICacheManager cacheManager)
            : base(graphManager)
        {
            _nodeManager = nodeManager;
            _graphEditor = graphEditor;
            _graphEventMonitor = graphEventMonitor;
            _cacheManager = cacheManager;
        }


        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(
            IGraphContext graphContext,
            IMindSettings settings = null)
        {
            var descriptor = _graphManager.FindGraph(graphContext);
            MakeSettings(ref settings);

            return MakeGraph(
                graphContext,
                () =>
                {
                    var graph = _graphEditor.GraphFactory<int>();

                    // This won't include nodes that are not connected to anything
                    graph.AddVerticesAndEdgeRange(
                        descriptor.PathServices.ConnectionManager.GetAll(graphContext)
                            .Select(connector => new UndirectedEdge<int>(connector.Node1Id, connector.Node2Id)));

                    return graph;
                },
                settings,
                "WholeGraph");
        }

        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(
            IGraphContext graphContext,
            IEnumerable<IContent> nodes,
            IMindSettings settings = null)
        {
            if (nodes == null) throw new ArgumentNullException("nodes");

            var nodeCount = nodes.Count();
            if (nodeCount == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            var descriptor = _graphManager.FindGraph(graphContext);
            MakeSettings(ref settings);

            // If there's only one node, return its neighbours
            if (nodeCount == 1)
            {
                return GetNeighboursGraph(graphContext, descriptor, nodes.First(), settings);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (settings.Algorithm == MindAlgorithm.Simple)
            {
                return MakeSimpleAssociations(graphContext, descriptor, nodes, settings);
            }
            // Calculate the routes between two nodes
            else
            {
                return MakeSophisticatedAssociations(graphContext, descriptor, nodes, settings);
            }
        }

        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetPartialGraph(
            IGraphContext graphContext,
            IContent centerNode,
            IMindSettings settings = null)
        {
            if (centerNode == null) throw new ArgumentNullException("centerNode");

            var descriptor = _graphManager.FindGraph(graphContext);
            MakeSettings(ref settings);

            return MakeGraph(
                graphContext,
                () =>
                {
                    return descriptor.PathServices.PathFinder.FindPaths(graphContext, centerNode.Id, -1, settings.MaxDistance).TraversedGraph;
                },
                settings,
                "PartialGraph." + centerNode.Id.ToString());
        }


        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetNeighboursGraph(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IContent node,
            IMindSettings settings)
        {
            return MakeGraph(
                graphContext,
                () =>
                {
                    var graph = _graphEditor.GraphFactory<int>();

                    graph.AddVertex(node.Id);
                    graph.AddVerticesAndEdgeRange(
                        descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, node.Id)
                            .Select(neighbourId => new UndirectedEdge<int>(node.Id, neighbourId)));

                    return graph;
                },
                settings,
                "NeighboursGraph." + node.Id.ToString());
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeSimpleAssociations(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IEnumerable<IContent> nodes,
            IMindSettings settings)
        {
            return MakeGraph(
                graphContext,
                () =>
                {
                    // Simply calculate the intersection of the neighbours of the nodes
                    var graph = _graphEditor.GraphFactory<int>();

                    var commonNeighbourIds = descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, nodes.First().Id);
                    var remainingNodes = new List<IContent>(nodes); // Maybe later we will need all the searched nodes
                    remainingNodes.RemoveAt(0);
                    commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, node.Id)).ToList());
                    // Same as
                    //foreach (var node in remainingNodes)
                    //{
                    //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList();
                    //}

                    foreach (var node in nodes)
                    {
                        foreach (var neighbourId in commonNeighbourIds)
                        {
                            graph.AddVerticesAndEdge(new UndirectedEdge<int>(node.Id, neighbourId));
                        }
                    }

                    return graph;
                },
                settings,
                "SimpleAssociations." + String.Join(", ", nodes.Select(node => node.Id.ToString())));
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

            return MakeGraph(
                graphContext,
                () =>
                {
                    var graph = _graphEditor.GraphFactory<int>();
                    IList<IEnumerable<int>> succeededPaths;

                    var allPairSucceededPaths = descriptor.PathServices.PathFinder.FindPaths(graphContext, nodeList[0].Id, nodeList[1].Id, settings.MaxDistance, settings.UseCache).SucceededPaths;

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
                                var pairSucceededPaths = descriptor.PathServices.PathFinder.FindPaths(graphContext, nodeList[i].Id, nodeList[n].Id, settings.MaxDistance, settings.UseCache).SucceededPaths;
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

                    graph.AddVertexRange(getSucceededNodeIds(succeededPaths));

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
                            if (!graph.TryGetEdge(node2Id, node1Id, out reversedNewEdge))
                            {
                                graph.AddEdge(newEdge);
                            }
                        }
                    }

                    return graph;
                },
                settings,
                "SophisticatedAssociations." + String.Join(", ", nodes.Select(node => node.Id.ToString())));
        }

        protected IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeGraph(
            IGraphContext graphContext,
            Func<IUndirectedGraph<int, IUndirectedEdge<int>>> createIdGraph,
            IMindSettings settings,
            string cacheKey)
        {
            if (settings.UseCache)
            {
                cacheKey = MakeCacheKey(graphContext.GraphName + "." + cacheKey + ".MindSettings:" + settings.Algorithm + settings.MaxDistance + ".Zoom:" + settings.ZoomLevel + "/" + settings.ZoomLevelCount);
                var zoomedGraph = _cacheManager.Get(cacheKey, ctx =>
                {
                    _graphEventMonitor.MonitorChanged(graphContext, ctx);
                    return _graphEditor.CreateZoomedGraph<int>(createIdGraph(), settings.ZoomLevel, settings.ZoomLevelCount);
                });

                return MakeContentGraph(graphContext, zoomedGraph, settings);
            }
            else return MakeContentGraph(graphContext, _graphEditor.CreateZoomedGraph<int>(createIdGraph(), settings.ZoomLevel, settings.ZoomLevelCount), settings);
        }


        protected IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph, IMindSettings settings)
        {
            var query = _nodeManager.GetManyContentQuery(graphContext, idGraph.Vertices);
            settings.ModifyQuery(query);
            var nodes = query.List().ToDictionary(node => node.Id);

            var graph = _graphEditor.GraphFactory<IContent>();
            graph.AddVertexRange(nodes.Values);

            foreach (var edge in idGraph.Edges)
            {
                // Since the QueryModifier could have removed items, this check is necessary
                if (nodes.ContainsKey(edge.Source) && nodes.ContainsKey(edge.Target))
                {
                    graph.AddEdge(new UndirectedEdge<IContent>(nodes[edge.Source], nodes[edge.Target]));
                }
            }

            return graph;
        }

        protected static string MakeCacheKey(string name)
        {
            return _cachePrefix + name;
        }

        protected void MakeSettings(ref IMindSettings settings)
        {
            if (settings == null) settings = new MindSettings();
        }
    }
}