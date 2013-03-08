using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard.Caching;
using Associativy.Models.Nodes;
using System.Threading;

namespace Associativy.Services
{
    /// <summary>
    /// Connections manager storing the graph in memory (i.e. not persisting it)
    /// </summary>
    public class MemoryConnectionManager : GraphServiceBase, IMemoryConnectionManager
    {
        protected readonly ICacheManager _cacheManager;
        protected readonly IGraphEventHandler _graphEventHandler;

        /// <summary>
        /// Stores the connections.
        /// Schema for connections: [graphName]->Graph.Connections[node1Id][node2Id] = dummy. This aims fast lookup of connections between two nodes.
        /// </summary>
        /// <remarks>
        /// Race conditions could occur, revise if necessary.
        /// </remarks>
        protected ConcurrentDictionary<string, Graph> Storage
        {
            get
            {
                return _cacheManager.Get("Associativy.GraphStorage", ctx =>
                {
                    return new ConcurrentDictionary<string, Graph>();
                });
            }
        }


        public MemoryConnectionManager(
            IGraphDescriptor graphDescriptor,
            ICacheManager cacheManager,
            IGraphEventHandler graphEventHandler)
            : base(graphDescriptor)
        {
            _cacheManager = cacheManager;
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

            Action<int, int> storeConnection =
                (id1, id2) =>
                {
                    var subDictionary = graph.Connections.GetOrAdd(id1, new ConcurrentDictionary<int, byte>());
                    subDictionary[id2] = 0;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id);
            storeConnection(node2Id, node1Id);
            Interlocked.Increment(ref graph.ConnectionCount);

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
            }

            _graphEventHandler.ConnectionsDeletedFromNode(_graphDescriptor, nodeId);
        }

        public void Disconnect(int node1Id, int node2Id)
        {
            if (!AreNeighbours(node1Id, node2Id)) return;

            var graph = GetGraph();

            byte dummyValue;
            graph.Connections[node1Id].TryRemove(node2Id, out dummyValue);
            graph.Connections[node2Id].TryRemove(node1Id, out dummyValue);

            Interlocked.Decrement(ref graph.ConnectionCount);

            _graphEventHandler.ConnectionDeleted(_graphDescriptor, node1Id, node2Id);
        }

        public IEnumerable<INodeToNodeConnector> GetAll(int skip, int count)
        {
            var addedConnections = new HashSet<string>();
            var connectors = new List<INodeToNodeConnector>();

            foreach (var connections in GetGraph().Connections)
            {
                foreach (var connection in connections.Value)
                {
                    if (!addedConnections.Contains(connections.Key + "-" + connection.Key) && !addedConnections.Contains(connection.Key + "-" + connections.Key))
                    {
                        connectors.Add(new NodeConnector { Node1Id = connections.Key, Node2Id = connection.Key });
                        addedConnections.Add(connections.Key + "-" + connection.Key);
                    }
                }
            }

            return connectors.Cast<INodeToNodeConnector>();
        }

        public IEnumerable<int> GetNeighbourIds(int nodeId, int skip, int count)
        {
            ConcurrentDictionary<int, byte> subDictionary;

            if (GetGraph().Connections.TryGetValue(nodeId, out subDictionary)) return subDictionary.Keys;

            return Enumerable.Empty<int>();
        }

        public int GetNeighbourCount(int nodeId)
        {
            return GetNeighbourIds(nodeId, 0, int.MaxValue).Count();
        }


        private Graph GetGraph()
        {
            return Storage.GetOrAdd(_graphDescriptor.Name, new Graph());
        }


        protected class Graph
        {
            public int ConnectionCount;
            public ConcurrentDictionary<int, ConcurrentDictionary<int, byte>> Connections;

            public Graph()
            {
                ConnectionCount = 0;
                Connections = new ConcurrentDictionary<int, ConcurrentDictionary<int, byte>>();
            }
        }
    }
}