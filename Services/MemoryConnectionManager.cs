using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Associativy.Models.Services;
using Orchard.Caching.Services;

namespace Associativy.Services
{
    /// <summary>
    /// Connections manager storing the graph in memory (i.e. not persisting it)
    /// </summary>
    public class MemoryConnectionManager : GraphAwareServiceBase, IMemoryConnectionManager
    {
        protected readonly ICacheService _cacheService;
        protected readonly IGraphEventHandler _graphEventHandler;

        /// <summary>
        /// Stores the connections.
        /// Schema for connections: [graphName]->Graph.Connections[node1Id][node2Id] = 0/1 (even/odd). This aims fast lookup of connections between two nodes.
        /// </summary>
        /// <remarks>
        /// Race conditions could occur, revise if necessary.
        /// </remarks>
        protected ConcurrentDictionary<string, Graph> Storage
        {
            get
            {
                return _cacheService.Get("Associativy.MemoryConnectionManager.GraphStorage", () =>
                {
                    return new ConcurrentDictionary<string, Graph>();
                });
            }
        }


        public MemoryConnectionManager(
            IGraphDescriptor graphDescriptor,
            ICacheService cacheService,
            IGraphEventHandler graphEventHandler)
            : base(graphDescriptor)
        {
            _cacheService = cacheService;
            _graphEventHandler = graphEventHandler;
        }


        public int GetConnectionCount()
        {
            return GetGraph().ConnectionCount;
        }

        public bool AreNeighbours(int node1Id, int node2Id)
        {
            if (node1Id == node2Id) return true;

            ConcurrentDictionary<int, byte> subDictionary;

            if (GetGraph().Connections.TryGetValue(node1Id, out subDictionary))
            {
                return subDictionary.ContainsKey(node2Id);
            }

            return false;
        }

        public void Connect(int node1Id, int node2Id)
        {
            if (AreNeighbours(node1Id, node2Id)) return;

            var graph = GetGraph();

            Action<int, int, byte> storeConnection =
                (id1, id2, parity) =>
                {
                    var subDictionary = graph.Connections.GetOrAdd(id1, new ConcurrentDictionary<int, byte>());
                    subDictionary[id2] = parity;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id, 0);
            storeConnection(node2Id, node1Id, 1);
            Interlocked.Increment(ref graph.ConnectionCount);

            var node1NeighbourCount = GetNeighbourCount(node1Id);
            if (node1NeighbourCount > graph.BiggestNodeNeighbourCount)
            {
                graph.BiggestNodeId = node1Id;
                graph.BiggestNodeNeighbourCount = node1NeighbourCount;
            }
            else
            {
                var node2NeighbourCount = GetNeighbourCount(node2Id);
                if (node2NeighbourCount > graph.BiggestNodeNeighbourCount)
                {
                    graph.BiggestNodeId = node2Id;
                    graph.BiggestNodeNeighbourCount = node2NeighbourCount;
                }
            }

            _graphEventHandler.ConnectionAdded(_graphDescriptor, node1Id, node2Id);
        }

        public void DeleteFromNode(int nodeId)
        {
            var graph = GetGraph();
            ConcurrentDictionary<int, byte> subDictionary;
            if (graph.Connections.TryRemove(nodeId, out subDictionary))
            {
                byte dummyValue;
                foreach (var neighbourId in subDictionary.Keys)
                {
                    graph.Connections[neighbourId].TryRemove(nodeId, out dummyValue);
                    Interlocked.Decrement(ref graph.ConnectionCount);
                }

                _graphEventHandler.ConnectionsDeletedFromNode(_graphDescriptor, nodeId);

                if (graph.BiggestNodeId == nodeId) FindBiggestNode(graph);
            }
        }

        public void Disconnect(int node1Id, int node2Id)
        {
            if (!AreNeighbours(node1Id, node2Id)) return;

            var graph = GetGraph();

            byte dummyValue;
            graph.Connections[node1Id].TryRemove(node2Id, out dummyValue);
            graph.Connections[node2Id].TryRemove(node1Id, out dummyValue);

            Interlocked.Decrement(ref graph.ConnectionCount);

            if (graph.BiggestNodeId == node1Id || graph.BiggestNodeId == node2Id) FindBiggestNode(graph);

            _graphEventHandler.ConnectionDeleted(_graphDescriptor, node1Id, node2Id);
        }

        public IEnumerable<INodeToNodeConnector> GetAll(int skip, int count)
        {
            return GetGraph().Connections
                    .SelectMany(subDictionaryKvp => subDictionaryKvp.Value
                        .Where(kvp => kvp.Value == 0)
                        .Select(kvp => new Associativy.Models.Nodes.NodeConnector { Node1Id = subDictionaryKvp.Key, Node2Id = kvp.Key })
                        )
                    .Skip(skip)
                    .Take(count);
        }

        public IEnumerable<int> GetNeighbourIds(int nodeId, int skip, int count)
        {
            ConcurrentDictionary<int, byte> subDictionary;

            if (GetGraph().Connections.TryGetValue(nodeId, out subDictionary)) return subDictionary.Keys.Skip(skip).Take(count);

            return Enumerable.Empty<int>();
        }

        public int GetNeighbourCount(int nodeId)
        {
            ConcurrentDictionary<int, byte> subDictionary;
            if (GetGraph().Connections.TryGetValue(nodeId, out subDictionary)) return subDictionary.Count;

            return 0;
        }

        public IGraphInfo GetGraphInfo()
        {
            return new GraphInfo
            {
                NodeCountLazy = new Lazy<int>(() => GetGraph().Connections.Count),
                ConnectionCountLazy = new Lazy<int>(() => GetGraph().ConnectionCount),
                CentralNodeIdLazy = new Lazy<int>(() => GetGraph().BiggestNodeId)
            };
        }


        protected Graph GetGraph()
        {
            return Storage.GetOrAdd(_graphDescriptor.Name, new Graph());
        }


        protected static void FindBiggestNode(Graph graph)
        {
            var nodeKvp = graph.Connections.Aggregate((node1, node2) => node1.Value.Count > node2.Value.Count ? node1 : node2);
            graph.BiggestNodeId = nodeKvp.Key;
            graph.BiggestNodeNeighbourCount = nodeKvp.Value.Count;
        }


        protected class Graph
        {
            public int ConnectionCount;
            public int BiggestNodeId;
            public int BiggestNodeNeighbourCount;

            // Schema: [node1Id][node2Id] = dummy, stored both ways for fast lookup
            public ConcurrentDictionary<int, ConcurrentDictionary<int, byte>> Connections;

            public Graph()
            {
                ConnectionCount = 0;
                BiggestNodeNeighbourCount = 0;
                Connections = new ConcurrentDictionary<int, ConcurrentDictionary<int, byte>>();
            }
        }


        protected class GraphInfo : IGraphInfo
        {
            public Lazy<int> NodeCountLazy { get; set; }
            public int NodeCount { get { return NodeCountLazy.Value; } }
            public Lazy<int> ConnectionCountLazy { get; set; }
            public int ConnectionCount { get { return ConnectionCountLazy.Value; } }
            public Lazy<int> CentralNodeIdLazy { get; set; }
            public int CentralNodeId { get { return CentralNodeIdLazy.Value; } }
        }
    }
}