using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Events;
using Associativy.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using Orchard.Services;
using QuickGraph;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    // Önmagában csak a TNodePart kellene
    public class Mind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IConnectionManager<TNodeToNodeConnectorRecord> _connectionManager;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;
        
        #region Caching fields
        protected readonly ICacheManager _cacheManager;
        protected readonly ISignals _signals;
        protected readonly string CachePrefix = "Associativy." + typeof(TNodePart).Name;
        protected readonly string GraphSignal = "Associativy.Graph." + typeof(TNodePart).Name;
        #endregion

        public Mind(
            IConnectionManager<TNodeToNodeConnectorRecord> connectionManager,
            INodeManager<TNodePart, TNodePartRecord> nodeManager,
            ICacheManager cacheManager,
            ISignals signals)
        {
            _connectionManager = connectionManager;
            _nodeManager = nodeManager;

            _cacheManager = cacheManager;
            _signals = signals;

            connectionManager.GraphChanged += TriggerGraphChangedSignal;
            nodeManager.GraphChanged += TriggerGraphChangedSignal;
        }

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociations(int zoomLevel = 0, bool useCache = true)
        {
            if (useCache)
            {
                return _cacheManager.Get(MakeCacheKey("WholeGraph"), ctx =>
                    {
                        MonitorGraphChangedSignal(ctx);
                        return GetAllAssociations(zoomLevel, false);
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

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(
            IList<TNodePart> nodes, 
            bool simpleAlgorithm = false, 
            int zoomLevel = 0, 
            bool useCache = true)
        {
            if (useCache)
            {
                string cacheKey = "";
                nodes.ToList().ForEach(node => cacheKey += node.Id.ToString() + ", ");
                return _cacheManager.Get(MakeCacheKey(cacheKey), ctx =>
                    {
                        MonitorGraphChangedSignal(ctx);
                        return MakeAssociations(nodes, simpleAlgorithm, zoomLevel, false);
                    });
            }

            if (nodes == null) throw new ArgumentNullException("The list of searched nodes can't be empty");
            if (nodes.Count == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            // If there's only one node, return its neighbours
            if (nodes.Count == 1)
            {
                return GetNeighboursGraph(nodes[0]);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (simpleAlgorithm)
            {
                return MakeSimpleAssocations(nodes);
            }
            // Calculate the routes between two nodes
            else
            {
                return MakeSophisticatedAssociations(nodes);
            }
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetNeighboursGraph(TNodePart node)
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

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSimpleAssocations(IList<TNodePart> nodes)
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

            if (commonNeighbourIds.Count == 0) return null;

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

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSophisticatedAssociations(IList<TNodePart> nodes)
        {
            if (nodes.Count < 2) throw new ArgumentException("The count of nodes should be at least two.");

            var graph = GraphFactory();
            List<List<int>> succeededPaths;

            var allPairSucceededPaths = CalculatePaths(nodes[0].Id, nodes[1].Id);

            if (allPairSucceededPaths.Count == 0) return null;

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
                        // Itt lehetne multithreading
                        var pairSucceededPaths = CalculatePaths(nodes[i].Id, nodes[n].Id);
                        commonSucceededNodeIds = commonSucceededNodeIds.Intersect(GetSucceededNodeIds(pairSucceededPaths).Union(searchedNodeIds)).ToList();
                        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();

                        n++;
                    }
                }

                if (allPairSucceededPaths.Count == 0 || commonSucceededNodeIds.Count == 0) return null;

                succeededPaths = new List<List<int>>(allPairSucceededPaths.Count); // We are oversizing, but it's worth the performance gain

                foreach (var path in allPairSucceededPaths)
                {
                    var succeededPath = path.Intersect(commonSucceededNodeIds);
                    if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath.ToList()); // Only paths where intersecting nodes are present
                }

                if (succeededPaths.Count == 0) return null;
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

        private Dictionary<int, TNodePart> GetSucceededNodes(List<List<int>> succeededPaths)
        {
            return _nodeManager.GetMany(GetSucceededNodeIds(succeededPaths)).ToDictionary(node => node.Id);
        }

        private List<int> GetSucceededNodeIds(List<List<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>(succeededPaths.Count); // An incorrect estimate, but enhaces performance
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList()); // To parallel?
            return succeededNodeIds;
        }

        #region CalculatePaths() auxiliary classes
        private class PathNode
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

        private class FrontierNode
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

        private List<List<int>> CalculatePaths(int startId, int targetId, int maxDistance = 3, bool useCache = true)
        {
            if (useCache)
            {
                return _cacheManager.Get(MakeCacheKey(startId.ToString() + targetId.ToString() + maxDistance.ToString()), ctx =>
                {
                    MonitorGraphChangedSignal(ctx);
                    return CalculatePaths(startId, targetId, maxDistance, false);
                });
            }

            var explored = new Dictionary<int, PathNode>();
            var succeededPaths = new List<List<int>>();
            var frontier = new Stack<FrontierNode>();

            explored[startId] = new PathNode(startId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startId] });

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
                if (currentDistance == maxDistance - 1)
                {
                    // Target will be only found if it's the direct neighbour of current
                    if (_connectionManager.AreNeighbours(currentNode.Id, targetId))
                    {
                        if (!explored.ContainsKey(targetId)) explored[targetId] = new PathNode(targetId);
                        if (explored[targetId].MinDistance > currentDistance + 1)
                        {
                            explored[targetId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetId]);
                        currentPath.Add(targetId);
                        succeededPaths.Add(currentPath);
                    }
                }
                // We can traverse the graph further
                else
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        var neighbourIds = _connectionManager.GetNeighbourIds(currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count);
                        foreach (var neighbourId in neighbourIds)
                        {
                            if (!explored.ContainsKey(neighbourId))
                            {
                                explored[neighbourId] = new PathNode(neighbourId);
                            }
                            currentNode.Neighbours.Add(explored[neighbourId]);
                        }
                    }

                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        // Target is a neighbour
                        if (neighbour.Id == targetId)
                        {
                            var succeededPath = new List<int>(currentPath) { targetId }; // Since we will use currentPath in further iterations too
                            succeededPaths.Add(succeededPath);
                        }
                        // We can traverse further, push the neighbour onto the stack
                        else if (neighbour.Id != startId)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                            frontier.Push(new FrontierNode { Distance = currentDistance + 1, Path = new List<int>(currentPath), Node = neighbour });
                        }

                        // If this is the shortest path to the node, overwrite its minDepth
                        if (neighbour.Id != startId && neighbour.MinDistance > currentDistance + 1)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                        }
                    }
                }
            }


            return succeededPaths;
        }

        private void MonitorGraphChangedSignal(AcquireContext<string> ctx)
        {
            ctx.Monitor(_signals.When(GraphSignal));
        }

        private void TriggerGraphChangedSignal(object sender, GraphEventArgs e)
        {
            _signals.Trigger(GraphSignal);
        }

        private string MakeCacheKey(string name)
        {
            return CachePrefix + name;
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GraphFactory()
        {
            return new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();
        }
    }
}