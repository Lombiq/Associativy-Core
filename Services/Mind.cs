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
        protected readonly IMindEventHandler _eventHandler;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected const string _cachePrefix = "Associativy.";
        #endregion


        public Mind(
            IGraphManager graphManager,
            INodeManager nodeManager,
            IGraphEditor graphEditor,
            IGraphEventMonitor graphEventMonitor,
            IMindEventHandler eventHandler,
            ICacheManager cacheManager)
            : base(graphManager)
        {
            _nodeManager = nodeManager;
            _graphEditor = graphEditor;
            _graphEventMonitor = graphEventMonitor;
            _eventHandler = eventHandler;
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

                    _eventHandler.BeforeWholeContentGraphBuilding(new BeforeWholeContentGraphBuildingContext(graphContext, settings, graph));

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
            else if (settings.Algorithm == "simple")
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
                    var graph = descriptor.PathServices.PathFinder.FindPaths(graphContext, centerNode.ContentItem.Id, -1, settings.MaxDistance).TraversedGraph;
                    _eventHandler.BeforePartialContentGraphBuilding(new BeforePartialContentGraphBuildingContext(graphContext, settings, centerNode, graph));
                    return graph;
                },
                settings,
                "PartialGraph." + centerNode.ContentItem.Id.ToString());
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

                    graph.AddVertex(node.ContentItem.Id);
                    graph.AddVerticesAndEdgeRange(
                        descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, node.ContentItem.Id)
                            .Select(neighbourId => new UndirectedEdge<int>(node.ContentItem.Id, neighbourId)));

                    _eventHandler.BeforeSearchedContentGraphBuilding(new BeforeSearchedContentGraphBuildingContext(graphContext, settings, new IContent[] { node }, graph));

                    return graph;
                },
                settings,
                "NeighboursGraph." + node.ContentItem.Id.ToString());
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

                    var commonNeighbourIds = descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, nodes.First().ContentItem.Id);
                    var remainingNodes = new List<IContent>(nodes); // Maybe later we will need all the searched nodes
                    remainingNodes.RemoveAt(0);
                    commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(descriptor.PathServices.ConnectionManager.GetNeighbourIds(graphContext, node.ContentItem.Id)).ToList());
                    // Same as
                    //foreach (var node in remainingNodes)
                    //{
                    //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.ContentItem.Id)).ToList();
                    //}

                    foreach (var node in nodes)
                    {
                        foreach (var neighbourId in commonNeighbourIds)
                        {
                            graph.AddVerticesAndEdge(new UndirectedEdge<int>(node.ContentItem.Id, neighbourId));
                        }
                    }

                    _eventHandler.BeforeSearchedContentGraphBuilding(new BeforeSearchedContentGraphBuildingContext(graphContext, settings, nodes, graph));

                    return graph;
                },
                settings,
                "SimpleAssociations." + String.Join(", ", nodes.Select(node => node.ContentItem.Id.ToString())));
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeSophisticatedAssociations(
            IGraphContext graphContext,
            GraphDescriptor descriptor,
            IEnumerable<IContent> nodes,
            IMindSettings settings)
        {
            var nodeList = nodes.ToList();
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

            return MakeGraph(
                graphContext,
                () =>
                {
                    #region Experimental graph merging
                    //// Non-fully working
                    //// Needs proper paired merging
                    //var pathFinder = descriptor.PathServices.PathFinder;
                    //var resultGraphs = new List<IUndirectedGraph<int, IUndirectedEdge<int>>>(nodeCount);
                    //var graph = _graphEditor.GraphFactory<int>();
                    //var nodeIds = nodeList.Select(node => node.ContentItem.Id);
                    //var commonVertices = new List<int>(nodeIds);

                    //Func<IUndirectedGraph<int, IUndirectedEdge<int>>> emptyResult =
                    //    () =>
                    //    {
                    //        _eventHandler.BeforeSearchedContentGraphBuilding(graphContext, nodes, graph);
                    //        return graph;
                    //    };


                    //for (int i = 0; i < nodeCount - 1; i++)
                    //{
                    //    var node1 = nodeList[i].ContentItem.Id;
                    //    var node2 = nodeList[i + 1].ContentItem.Id;
                    //    var resultGraph = pathFinder.FindPaths(graphContext, node1, node2, settings.MaxDistance, settings.UseCache).SucceededGraph;

                    //    if (resultGraph.VertexCount == 0 || resultGraph.EdgeCount == 0) return emptyResult();

                    //    if (commonVertices.Count == 0) commonVertices.AddRange(resultGraph.Vertices);
                    //    else
                    //    {
                    //        commonVertices = commonVertices.Intersect(resultGraph.Vertices.Union(nodeIds)).ToList();
                    //        if (commonVertices.Count == 0) return emptyResult();
                    //    }

                    //    resultGraphs.Add(resultGraph);
                    //}

                    //graph.AddVertexRange(commonVertices);

                    //for (int i = 0; i < resultGraphs.Count; i++)
                    //{
                    //    var rawResultGraph = resultGraphs[i];
                    //    var resultGraph = _graphEditor.GraphFactory<int>();
                    //    resultGraphs[i] = resultGraph;
                    //    resultGraph.AddVertexRange(commonVertices); // This is more than needed, but causes no problem
                    //    resultGraph.AddEdgeRange(rawResultGraph.Edges.Where(edge => commonVertices.Contains(edge.Source) && commonVertices.Contains(edge.Target)));

                    //    // Here we remove every vertex that is not among the common ones. Where a vertex has only two edges (i.e. there's a path through it,
                    //    // that only goes through it) the two neighbouring nodes will be directly connected, if not, the vertex is removed with all of its
                    //    // connections.
                    //    foreach (var vertex in rawResultGraph.Vertices.Where(vertex => !commonVertices.Contains(vertex)))
                    //    {
                    //        if (rawResultGraph.AdjacentDegree(vertex) == 2)
                    //        {
                    //            var edges = rawResultGraph.AdjacentEdges(vertex);
                    //            var firstEdge = edges.First();
                    //            var secondEdge = edges.Last();
                    //            var vertex1 = firstEdge.Target == vertex ? firstEdge.Source : firstEdge.Target;
                    //            var vertex2 = secondEdge.Target == vertex ? secondEdge.Source : secondEdge.Target;
                    //            resultGraph.AddEdge(new UndirectedEdge<int>(vertex1, vertex2));
                    //        }
                    //    }
                    //}

                    //// Distilling edges to only keep common ones.
                    //var commonEdges = new List<IUndirectedEdge<int>>();
                    //commonEdges.AddRange(resultGraphs.SelectMany(resultGraph => resultGraph.Edges));

                    //for (int i = 0; i < commonEdges.Count; i++)
                    //{
                    //    var current = commonEdges[i];
                    //    var equivalentEdges = commonEdges.Where(edge => edge.Target == current.Target && edge.Source == current.Source
                    //                                            || edge.Source == current.Target && edge.Target == current.Source).ToList();
                    //    if (equivalentEdges.Count != nodeCount)
                    //    {
                    //        foreach (var edge in equivalentEdges)
                    //        {
                    //            commonEdges.Remove(edge);
                    //        }
                    //    }
                    //}

                    //graph.AddEdgeRange(commonEdges);

                    //_eventHandler.BeforeSearchedContentGraphBuilding(graphContext, nodes, graph);

                    //return graph;
                    #endregion

                    var graph = _graphEditor.GraphFactory<int>();
                    IList<IEnumerable<int>> succeededPaths;

                    Func<IUndirectedGraph<int, IUndirectedEdge<int>>> emptyResult =
                        () =>
                        {
                            _eventHandler.BeforeSearchedContentGraphBuilding(new BeforeSearchedContentGraphBuildingContext(graphContext, settings, nodes, graph));
                            return graph;
                        };

                    var allPairSucceededPaths = descriptor.PathServices.PathFinder.FindPaths(graphContext, nodeList[0].Id, nodeList[1].Id, settings.MaxDistance, settings.UseCache).SucceededPaths;

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


                        if (allPairSucceededPaths.Count() == 0 || commonSucceededNodeIds.Count == 0) return emptyResult();

                        succeededPaths = new List<IEnumerable<int>>(allPairSucceededPaths.Count()); // We are oversizing, but it's worth the performance gain

                        foreach (var path in allPairSucceededPaths)
                        {
                            var succeededPath = path.Intersect(commonSucceededNodeIds);
                            if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath); // Only paths where intersecting nodes are present
                        }


                        if (succeededPaths.Count() == 0) return emptyResult();
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

                    _eventHandler.BeforeSearchedContentGraphBuilding(new BeforeSearchedContentGraphBuildingContext(graphContext, settings, nodes, graph));

                    return graph;
                },
                settings,
                "SophisticatedAssociations." + String.Join(", ", nodes.Select(node => node.ContentItem.Id.ToString())));
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
            var query = _nodeManager.GetManyQuery(graphContext, idGraph.Vertices);
            var nodes = query.List().ToDictionary(node => node.Id);

            var graph = _graphEditor.GraphFactory<IContent>();
            graph.AddVertexRange(nodes.Values);

            foreach (var edge in idGraph.Edges)
            {
                // Since the query can be modified in an event handler and it could have removed items, this check is necessary
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