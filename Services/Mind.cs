using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using QuickGraph;
using Orchard;
using Associativy.Models.Mind;
using Associativy.EventHandlers;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class Mind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IConnectionManager<TNodeToNodeConnectorRecord> _connectionManager;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;
        protected readonly IPathFinder<TNodeToNodeConnectorRecord> _pathFinder;
        protected readonly IWorkContextAccessor _workContextAccessor;
        protected readonly IAssociativeGraphEventMonitor _associativeGraphEventMonitor;

        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected readonly string CachePrefix = "Associativy." + typeof(TNodePart).Name;
        protected readonly string GraphSignal = "Associativy.Graph." + typeof(TNodePart).Name;
        #endregion

        public Mind(
            IConnectionManager<TNodeToNodeConnectorRecord> connectionManager,
            INodeManager<TNodePart, TNodePartRecord> nodeManager,
            IPathFinder<TNodeToNodeConnectorRecord> pathFinder,
            IWorkContextAccessor workContextAccessor,
            IAssociativeGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
        {
            _connectionManager = connectionManager;
            _nodeManager = nodeManager;
            _pathFinder = pathFinder;
            _workContextAccessor = workContextAccessor;
            _associativeGraphEventMonitor = associativeGraphEventMonitor;

            _cacheManager = cacheManager;
        }

        public virtual IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> GetAllAssociations(IMindSettings settings = null)
        {
            MakeSettings(ref settings);

            if (settings.UseCache)
            {
                return _cacheManager.Get(MakeCacheKey("WholeGraph", settings), ctx =>
                    {
                        _associativeGraphEventMonitor.MonitorChangedSignal(ctx, GraphSignal);
                        settings.UseCache = false;
                        return GetAllAssociations(settings);
                    });
            }

            var graph = GraphFactory();

            var nodes = _nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id);

            foreach (var node in nodes)
            {
                graph.AddVertex(node.Value);
            }

            var connections = _connectionManager.GetAll();
            for (int i = 0; i < connections.Count; i++)
            {
                graph.AddEdge(new UndirectedEdge<TNodePart>(nodes[connections[i].Record1Id], nodes[connections[i].Record2Id]));
            }

            // Leaves out nodes that don't have any neighbours
            //foreach (var connection in _nodeManager.NodeToNodeRecordRepository.Table.ToList())
            //{
            //    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(nodes[connection.Record1Id], nodes[connection.Record2Id]));
            //}

            return graph;
        }

        public virtual IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> MakeAssociations(
            IList<TNodePart> nodes,
            IMindSettings settings = null)
        {
            MakeSettings(ref settings);

            if (settings.UseCache)
            {
                string cacheKey = "";
                nodes.ToList().ForEach(node => cacheKey += node.Id.ToString() + ", ");
                return _cacheManager.Get(MakeCacheKey(cacheKey, settings), ctx =>
                    {
                        _associativeGraphEventMonitor.MonitorChangedSignal(ctx, GraphSignal);
                        settings.UseCache = false;
                        return MakeAssociations(nodes, settings);
                    });
            }

            settings.UseCache = true;
            if (nodes == null) throw new ArgumentNullException("The list of searched nodes can't be empty");
            if (nodes.Count == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            // If there's only one node, return its neighbours
            if (nodes.Count == 1)
            {
                return GetNeighboursGraph(nodes[0]);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (settings.Algorithm == MindAlgorithms.Simple)
            {
                return MakeSimpleAssocations(nodes);
            }
            // Calculate the routes between two nodes
            else
            {
                return MakeSophisticatedAssociations(nodes, settings);
            }
        }

        private IMutableUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> GetNeighboursGraph(TNodePart node)
        {
            var graph = GraphFactory();

            graph.AddVertex(node);

            var nodes = _nodeManager.GetMany(_connectionManager.GetNeighbourIds(node.Id));
            for (int i = 0; i < nodes.Count; i++)
            {
                graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(node, nodes[i]));
            }

            return graph;
        }

        private IMutableUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> MakeSimpleAssocations(IList<TNodePart> nodes)
        {
            // Simply calculate the intersection of the neighbours of the nodes

            var graph = GraphFactory();

            var commonNeighbourIds = _connectionManager.GetNeighbourIds(nodes[0].Id);
            var remainingNodes = new List<TNodePart>(nodes); // Maybe later we will need all the searched nodes
            remainingNodes.RemoveAt(0);
            commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(_connectionManager.GetNeighbourIds(node.Id)).ToList());
            // Same as
            //foreach (var node in remainingNodes)
            //{
            //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList();
            //}

            if (commonNeighbourIds.Count == 0) return graph;

            var commonNeighbours = _nodeManager.GetMany(commonNeighbourIds);

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int n = 0; n < commonNeighbours.Count; n++)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(nodes[i], commonNeighbours[n]));
                }
            }

            return graph;
        }

        private IMutableUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> MakeSophisticatedAssociations(IList<TNodePart> nodes, IMindSettings settings)
        {
            if (nodes.Count < 2) throw new ArgumentException("The count of nodes should be at least two.");

            var graph = GraphFactory();
            IList<IList<int>> succeededPaths;

            var allPairSucceededPaths = _pathFinder.FindPaths(nodes[0], nodes[1], settings);

            if (allPairSucceededPaths.Count == 0) return graph;

            if (nodes.Count == 2)
            {
                succeededPaths = allPairSucceededPaths;
            }
            // Calculate the routes between every nodes pair, then calculate the intersection of the routes
            else
            {
                // We have to preserve the searched node ids in the succeeded paths despite the intersections
                var searchedNodeIds = new List<int>(nodes.Count);
                nodes.ToList().ForEach(
                        node => searchedNodeIds.Add(node.Id)
                    );

                var commonSucceededNodeIds = GetSucceededNodeIds(allPairSucceededPaths).Union(searchedNodeIds).ToList();

                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    int n = i + 1;
                    if (i == 0) n = 2; // Because of the calculation of intersections the first iteration is already done above

                    while (n < nodes.Count)
                    {
                        // Here could be multithreading
                        var pairSucceededPaths = _pathFinder.FindPaths(nodes[i], nodes[n], settings);
                        commonSucceededNodeIds = commonSucceededNodeIds.Intersect(GetSucceededNodeIds(pairSucceededPaths).Union(searchedNodeIds)).ToList();
                        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();

                        n++;
                    }
                }

                if (allPairSucceededPaths.Count == 0 || commonSucceededNodeIds.Count == 0) return graph;

                succeededPaths = new List<IList<int>>(allPairSucceededPaths.Count); // We are oversizing, but it's worth the performance gain

                foreach (var path in allPairSucceededPaths)
                {
                    var succeededPath = path.Intersect(commonSucceededNodeIds);
                    if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath.ToList()); // Only paths where intersecting nodes are present
                }

                if (succeededPaths.Count == 0) return graph;
            }


            var succeededNodes = GetSucceededNodes(succeededPaths);

            foreach (var path in succeededPaths)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(succeededNodes[path[i - 1]], succeededNodes[path[i]]));
                }
            }

            return graph;
        }

        private Dictionary<int, TNodePart> GetSucceededNodes(IList<IList<int>> succeededPaths)
        {
            return _nodeManager.GetMany(GetSucceededNodeIds(succeededPaths)).ToDictionary(node => node.Id);
        }

        private List<int> GetSucceededNodeIds(IList<IList<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>(succeededPaths.Count); // An incorrect estimate, but (micro)enhaces performance

            foreach (var row in succeededPaths)
            {
                succeededNodeIds = succeededNodeIds.Union(row).ToList();
            }

            return succeededNodeIds;
        }

        private string MakeCacheKey(string name, IMindSettings settings)
        {
            return MakeCacheKey(name)
                + "MindSettings:" + settings.Algorithm + settings.ZoomLevel + settings.MaxDistance;
        }

        private string MakeCacheKey(string name)
        {
            return CachePrefix + name;
        }

        private IMutableUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> GraphFactory()
        {
            return new UndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>();
        }

        private void MakeSettings(ref IMindSettings settings)
        {
            var workContext = _workContextAccessor.GetContext();
            if (settings == null) settings = workContext.Resolve<IMindSettings>();
        }
    }
}