using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using QuickGraph;

namespace Associativy.Services
{
    public interface IStandardMind : IMind, IDependency
    {
    }

    [OrchardFeature("Associativy")]
    public class StandardMind : GraphServiceBase, IStandardMind
    {
        protected readonly IGraphEditor _graphEditor;
        protected readonly IGraphEventMonitor _graphEventMonitor;
        protected readonly IMindEventHandler _eventHandler;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected const string _cachePrefix = "Associativy.";
        #endregion


        public StandardMind(
            IGraphDescriptor graphDescriptor,
            IGraphEditor graphEditor,
            IGraphEventMonitor graphEventMonitor,
            IMindEventHandler eventHandler,
            ICacheManager cacheManager)
            : base(graphDescriptor)
        {
            _graphEditor = graphEditor;
            _graphEventMonitor = graphEventMonitor;
            _eventHandler = eventHandler;
            _cacheManager = cacheManager;
        }


        public virtual IMindResult GetAllAssociations(IMindSettings settings)
        {
            MakeSettings(ref settings);

            return MakeResult(
                () =>
                {
                    var graph = _graphEditor.GraphFactory<int>();

                    // This won't include nodes that are not connected to anything
                    graph.AddVerticesAndEdgeRange(
                        _graphDescriptor.Services.ConnectionManager.GetAll()
                            .Select(connector => new UndirectedEdge<int>(connector.Node1Id, connector.Node2Id)));

                    _eventHandler.AllAssociationsGraphBuilt(new AllAssociationsGraphBuiltContext(_graphDescriptor, settings, graph));

                    return graph;
                },
                settings,
                "WholeGraph");
        }

        public virtual IMindResult MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings)
        {
            if (nodes == null) throw new ArgumentNullException("nodes");

            var nodeCount = nodes.Count();
            if (nodeCount == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            MakeSettings(ref settings);

            // If there's only one node, return its neighbours
            if (nodeCount == 1)
            {
                return GetNeighboursGraph(nodes.First(), settings);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (settings.Algorithm == "simple")
            {
                return MakeSimpleAssociations(nodes, settings);
            }
            // Calculate the routes between two nodes
            else
            {
                return MakeSophisticatedAssociations(nodes, settings);
            }
        }

        public virtual IMindResult GetPartialGraph(IContent centerNode, IMindSettings settings)
        {
            if (centerNode == null) throw new ArgumentNullException("centerNode");

            MakeSettings(ref settings);

            return MakeResult(
                () =>
                {
                    var graph = _graphDescriptor.Services.PathFinder.FindPaths(centerNode.ContentItem.Id, -1, new PathFinderSettings { MaxDistance = settings.MaxDistance }).TraversedGraph;
                    _eventHandler.PartialGraphBuilt(new PartialGraphBuiltContext(_graphDescriptor, settings, centerNode, graph));
                    return graph;
                },
                settings,
                "PartialGraph." + centerNode.ContentItem.Id.ToString());
        }


        protected virtual IMindResult GetNeighboursGraph(IContent node, IMindSettings settings)
        {
            return MakeResult(
                () =>
                {
                    var graph = _graphEditor.GraphFactory<int>();

                    graph.AddVertex(node.ContentItem.Id);
                    graph.AddVerticesAndEdgeRange(
                        _graphDescriptor.Services.ConnectionManager.GetNeighbourIds(node.ContentItem.Id)
                            .Take(settings.MaxNodeCount)
                            .Select(neighbourId => new UndirectedEdge<int>(node.ContentItem.Id, neighbourId)));

                    _eventHandler.SearchedGraphBuilt(new SearchedGraphBuiltContext(_graphDescriptor, settings, new IContent[] { node }, graph));

                    return graph;
                },
                settings,
                "NeighboursGraph." + node.ContentItem.Id.ToString());
        }

        protected virtual IMindResult MakeSimpleAssociations(IEnumerable<IContent> nodes, IMindSettings settings)
        {
            return MakeResult(
                () =>
                {
                    // Simply calculate the intersection of the neighbours of the nodes
                    var graph = _graphEditor.GraphFactory<int>();

                    var commonNeighbourIds = _graphDescriptor.Services.ConnectionManager.GetNeighbourIds(nodes.First().ContentItem.Id);
                    var remainingNodes = new List<IContent>(nodes); // Maybe later we will need all the searched nodes
                    remainingNodes.RemoveAt(0);
                    commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(_graphDescriptor.Services.ConnectionManager.GetNeighbourIds(node.ContentItem.Id)).ToList());
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

                    _eventHandler.SearchedGraphBuilt(new SearchedGraphBuiltContext(_graphDescriptor, settings, nodes, graph));

                    return graph;
                },
                settings,
                "SimpleAssociations." + String.Join(", ", nodes.Select(node => node.ContentItem.Id.ToString())));
        }

        protected virtual IMindResult MakeSophisticatedAssociations(IEnumerable<IContent> nodes, IMindSettings settings)
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

            return MakeResult(
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
                    //        _eventHandler.BeforeSearchedContentGraphBuilding(nodes, graph);
                    //        return graph;
                    //    };


                    //for (int i = 0; i < nodeCount - 1; i++)
                    //{
                    //    var node1 = nodeList[i].ContentItem.Id;
                    //    var node2 = nodeList[i + 1].ContentItem.Id;
                    //    var resultGraph = pathFinder.FindPaths(node1, node2, settings.MaxDistance, settings.UseCache).SucceededGraph;

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

                    //_eventHandler.BeforeSearchedContentGraphBuilding(nodes, graph);

                    //return graph;
                    #endregion

                    var graph = _graphEditor.GraphFactory<int>();
                    IList<IEnumerable<int>> succeededPaths;

                    Func<IUndirectedGraph<int, IUndirectedEdge<int>>> emptyResult =
                        () =>
                        {
                            _eventHandler.SearchedGraphBuilt(new SearchedGraphBuiltContext(_graphDescriptor, settings, nodes, graph));
                            return graph;
                        };

                    var pathFinderSettings = new PathFinderSettings { MaxDistance = settings.MaxDistance, UseCache = settings.UseCache };
                    var allPairSucceededPaths = _graphDescriptor.Services.PathFinder.FindPaths(nodeList[0].Id, nodeList[1].Id, pathFinderSettings).SucceededPaths;

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
                                var pairSucceededPaths = _graphDescriptor.Services.PathFinder.FindPaths(nodeList[i].Id, nodeList[n].Id, pathFinderSettings).SucceededPaths;
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

                    _eventHandler.SearchedGraphBuilt(new SearchedGraphBuiltContext(_graphDescriptor, settings, nodes, graph));

                    return graph;
                },
                settings,
                "SophisticatedAssociations." + String.Join(", ", nodes.Select(node => node.ContentItem.Id.ToString())));
        }

        protected IMindResult MakeResult(
            Func<IUndirectedGraph<int, IUndirectedEdge<int>>> createIdGraph,
            IMindSettings settings,
            string cacheKey)
        {
            return new MindResult(
                (mindResult) =>
                {
                    return createIdGraph();
                },
                (mindResult) =>
                {
                    if (settings.UseCache)
                    {
                        cacheKey = MakeCacheKey(_graphDescriptor.Name + "." + cacheKey + ".MindSettings:" + settings.Algorithm + settings.MaxDistance + ".Zoom:" + settings.ZoomLevel + "/" + settings.ZoomLevelCount);
                        return _cacheManager.Get(cacheKey, ctx =>
                        {
                            _graphEventMonitor.MonitorChanged(_graphDescriptor, ctx);
                            return _graphEditor.CreateZoomedGraph<int>(mindResult.UnzoomedIdGraph, settings.ZoomLevel, settings.ZoomLevelCount);
                        });
                    }

                    return _graphEditor.CreateZoomedGraph<int>(mindResult.UnzoomedIdGraph, settings.ZoomLevel, settings.ZoomLevelCount);
                },
                (mindResult) =>
                {
                    return _graphDescriptor.Services.NodeManager.MakeContentGraph(mindResult.IdGraph);
                },
                (mindResult) =>
                {
                    return _graphEditor.CalculateZoomLevelCount(mindResult.UnzoomedIdGraph, settings.ZoomLevelCount);
                });
        }


        protected static string MakeCacheKey(string name)
        {
            return _cachePrefix + name;
        }

        protected void MakeSettings(ref IMindSettings settings)
        {
            if (settings == null) settings = MindSettings.Default;
        }
    }
}