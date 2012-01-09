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

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class Mind : AssociativyService, IMind
    {
        protected readonly INodeManager _nodeManager;
        protected readonly IPathFinder _pathFinder;
        protected readonly IWorkContextAccessor _workContextAccessor;
        protected readonly IAssociativeGraphEventMonitor _associativeGraphEventMonitor;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected string _cachePrefix;
        #endregion

        private object _contextLocker = new object();
        public override IAssociativyContext Context
        {
            set
            {
                lock (_contextLocker) // This is to ensure that used services also have the same context
                {
                    _nodeManager.Context = value;
                    _pathFinder.Context = value;
                    _cachePrefix = value.TechnicalGraphName + ".";
                    base.Context = value; 
                }
            }
        }

        public Mind(
            IAssociativyContext associativyContext,
            INodeManager nodeManager,
            IPathFinder pathFinder,
            IWorkContextAccessor workContextAccessor,
            IAssociativeGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
            : base(associativyContext)
        {
            _nodeManager = nodeManager;
            _pathFinder = pathFinder;
            _workContextAccessor = workContextAccessor;
            _associativeGraphEventMonitor = associativeGraphEventMonitor;
            _cacheManager = cacheManager;
        }

        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(
            IMindSettings settings = null,
            Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null)
        {
            MakeSettings(ref settings);

            Func<IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>>> makeWholeGraph =
                () =>
                {
                    var wholeGraph = GraphFactory();

                    var query = queryModifier == null ? _nodeManager.ContentQuery : queryModifier(_nodeManager.ContentQuery);
                    var nodes = query.List().ToDictionary<IContent, int>(node => node.Id);

                    foreach (var node in nodes)
                    {
                        wholeGraph.AddVertex(node.Value);
                    }

                    var connections = Context.ConnectionManager.GetAll();
                    foreach (var connection in connections)
                    {
                        wholeGraph.AddEdge(new UndirectedEdge<IContent>(nodes[connection.Node1Id], nodes[connection.Node2Id]));
                    }

                    return wholeGraph;
                };


            if (settings.UseCache)
            {
                var graph = _cacheManager.Get(MakeCacheKey("WholeGraph"), ctx =>
                 {
                     _associativeGraphEventMonitor.MonitorChanged(ctx, Context);
                     return makeWholeGraph();
                 });

                return _cacheManager.Get(MakeCacheKey("WholeGraphZoomed.Zoom:" + settings.ZoomLevel, settings), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(ctx, Context);
                    return ZoomedGraph(graph, settings.ZoomLevel);
                });
            }
            else return ZoomedGraph(makeWholeGraph(), settings.ZoomLevel);
        }

        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(
            IEnumerable<IContent> nodes,
            IMindSettings settings = null,
            Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null)
        {
            if (nodes == null) throw new ArgumentNullException("The list of searched nodes can't be empty");

            var nodeCount = nodes.Count();
            if (nodeCount == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            MakeSettings(ref settings);

            if (queryModifier == null) queryModifier = (query) => query;

            Func<IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>>> makeGraph =
                () =>
                {
                    // If there's only one node, return its neighbours
                    if (nodeCount == 1)
                    {
                        return GetNeighboursGraph(nodes.First(), queryModifier);
                    }
                    // Simply calculate the intersection of the neighbours of the nodes
                    else if (settings.Algorithm == MindAlgorithms.Simple)
                    {
                        return MakeSimpleAssocations(nodes, queryModifier);
                    }
                    // Calculate the routes between two nodes
                    else
                    {
                        return MakeSophisticatedAssociations(nodes, settings, queryModifier);
                    }
                };

            if (settings.UseCache)
            {
                string cacheKey = "AssociativeGraph.";
                nodes.ToList().ForEach(node => cacheKey += node.Id.ToString() + ", ");

                var graph = _cacheManager.Get(MakeCacheKey(cacheKey, settings), ctx =>
                    {
                        _associativeGraphEventMonitor.MonitorChanged(ctx, Context);
                        return makeGraph();
                    });

                return _cacheManager.Get(MakeCacheKey(cacheKey + ".Zoom" + settings.ZoomLevel, settings), ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(ctx, Context);
                    return ZoomedGraph(graph, settings.ZoomLevel);
                });
            }
            else return ZoomedGraph(makeGraph(), settings.ZoomLevel);
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetNeighboursGraph(
            IContent node,
            Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier)
        {
            var graph = GraphFactory();

            graph.AddVertex(node);
            var neighbours = queryModifier(_nodeManager.GetManyQuery(Context.ConnectionManager.GetNeighbourIds(node.Id))).List();
            foreach (var neighbour in neighbours)
            {
                graph.AddVerticesAndEdge(new UndirectedEdge<IContent>(node, neighbour));
            }

            return graph;
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeSimpleAssocations(
            IEnumerable<IContent> nodes,
            Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier)
        {
            // Simply calculate the intersection of the neighbours of the nodes

            var graph = GraphFactory();

            var commonNeighbourIds = Context.ConnectionManager.GetNeighbourIds(nodes.First().Id);
            var remainingNodes = new List<IContent>(nodes); // Maybe later we will need all the searched nodes
            remainingNodes.RemoveAt(0);
            commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(Context.ConnectionManager.GetNeighbourIds(node.Id)).ToList());
            // Same as
            //foreach (var node in remainingNodes)
            //{
            //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList();
            //}

            if (commonNeighbourIds.Count() == 0) return graph;

            var commonNeighbours = queryModifier(_nodeManager.GetManyQuery(commonNeighbourIds)).List();

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
            IEnumerable<IContent> nodes,
            IMindSettings settings,
            Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier)
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


            var graph = GraphFactory();
            IList<IEnumerable<int>> succeededPaths;

            var allPairSucceededPaths = _pathFinder.FindPaths(nodeList[0].Id, nodeList[1].Id, settings);

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
                        var pairSucceededPaths = _pathFinder.FindPaths(nodeList[i].Id, nodeList[n].Id, settings);
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

            var succeededNodes = queryModifier(_nodeManager.GetManyQuery(getSucceededNodeIds((succeededPaths)))).List().ToDictionary(node => node.Id);

            foreach (var path in succeededPaths)
            {
                var pathList = path.ToList();
                for (int i = 1; i < pathList.Count; i++)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<IContent>(succeededNodes[pathList[i - 1]], succeededNodes[pathList[i]]));
                }
            }

            return graph;
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> ZoomedGraph(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevel)
        {
            // Caching doesn't work, fails at graph.AdjacentEdges(node), the graph can't find the object. Caused most likely
            // because the objects are not the same. But it seems that although the calculation to the last block is repeated
            // with the same numbers for a graph when calculating zoomed graphs, there is no gain in caching: the algorithm runs
            // freaking fast: ~100 ticks on i7@3,2Ghz, running sequentially. That's very far from even one ms...


            /// Grouping vertices by the number of their neighbours (= adjacentDegree)
            var nodes = graph.Vertices.ToList();
            var adjacentDegreeGroups = new SortedList<int, List<IContent>>();
            var maxAdjacentDegree = 0;
            foreach (var node in nodes)
            {
                var adjacentDegree = graph.AdjacentDegree(node);
                if (adjacentDegree > maxAdjacentDegree) maxAdjacentDegree = adjacentDegree;
                if (!adjacentDegreeGroups.ContainsKey(adjacentDegree)) adjacentDegreeGroups[adjacentDegree] = new List<IContent>();
                adjacentDegreeGroups[adjacentDegree].Add(node);
            }


            /// Partitioning nodes into continuous zoom levels
            int approxVerticesInPartition = (int)Math.Round((double)(nodes.Count / Context.MaxZoomLevel), 0);
            if (approxVerticesInPartition == 0) approxVerticesInPartition = nodes.Count; // Too little number of nodes
            int currentRealZoomLevel = 0;
            int previousRealZoomLevel = -1;
            int nodeCountTillThisLevel = 0;
            var zoomPartitions = new List<List<IContent>>(Context.MaxZoomLevel); // Nodes partitioned by zoom level, filled up continuously
            // Iterating backwards as nodes with higher neighbourCount are on the top
            // I.e.: with zoomlevel 0 only the nodes with the highest neighbourCount will be returned, on MaxZoomLevel
            // all the nodes.
            var reversedAdjacentDegreeGroups = adjacentDegreeGroups.Reverse();
            foreach (var nodeGroup in reversedAdjacentDegreeGroups)
            {
                nodeCountTillThisLevel += nodeGroup.Value.Count;
                currentRealZoomLevel = (int)Math.Floor((double)(nodeCountTillThisLevel / approxVerticesInPartition));

                if (previousRealZoomLevel != currentRealZoomLevel) zoomPartitions.Add(nodeGroup.Value); // We've reached a new zoom level
                else zoomPartitions[zoomPartitions.Count - 1].AddRange(nodeGroup.Value);

                previousRealZoomLevel = currentRealZoomLevel;
            }


            /// Removing all nodes that are above the specified zoom level
            IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> zoomedGraph = GraphFactory();
            zoomedGraph.AddVerticesAndEdgeRange(graph.Edges); // Copying the original graph

            int i = zoomPartitions.Count - 1;
            while (i >= 0 && i > zoomLevel)
            {
                foreach (var node in zoomPartitions[i])
                {
                    // Rewiring all edges so that nodes previously connected through this nodes now get directly connected
                    // Looks unneeded and wrong
                    //if (zoomedGraph.AdjacentDegree(node) > 1)
                    //{
                    //    foreach (var edge in zoomedGraph.AdjacentEdges(node))
                    //    {

                    //    }
                    //}
                    zoomedGraph.RemoveVertex(node);
                }

                i--;
            }


            return zoomedGraph;
        }

        protected virtual string MakeCacheKey(string name, IMindSettings settings)
        {
            return MakeCacheKey(name)
                + "MindSettings:" + settings.Algorithm + settings.MaxDistance;
        }

        protected virtual string MakeCacheKey(string name)
        {
            return _cachePrefix + name;
        }

        protected virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GraphFactory()
        {
            return new UndirectedGraph<IContent, IUndirectedEdge<IContent>>();
        }

        protected virtual void MakeSettings(ref IMindSettings settings)
        {
            var workContext = _workContextAccessor.GetContext();
            if (settings == null) settings = workContext.Resolve<IMindSettings>();
        }
    }
}