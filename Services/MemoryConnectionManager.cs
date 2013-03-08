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

            return GetNode(node1Id).Connections.ContainsKey(node2Id);
        }

        public void Connect(int node1Id, int node2Id)
        {
            if (AreNeighbours(node1Id, node2Id)) return;

            var graph = GetGraph();

            var node1 = GetNode(node1Id);
            var node2 = GetNode(node2Id);
            node1.Connections[node2Id] = node2;
            node2.Connections[node1Id] = node1;

            Interlocked.Increment(ref graph.ConnectionCount);

            _graphEventHandler.ConnectionAdded(_graphDescriptor, node1Id, node2Id);
        }

        public void DeleteFromNode(int nodeId)
        {
            var graph = GetGraph();

            Node node;
            Node dummy;
            if (graph.Nodes.TryRemove(nodeId, out node))
            {
                foreach (var neighbourId in node.Connections.Keys)
                {
                    graph.Nodes[neighbourId].Connections.TryRemove(nodeId, out dummy);
                    Interlocked.Decrement(ref graph.ConnectionCount);
                }
            }

            _graphEventHandler.ConnectionsDeletedFromNode(_graphDescriptor, nodeId);
        }

        public void Disconnect(int node1Id, int node2Id)
        {
            if (!AreNeighbours(node1Id, node2Id)) return;

            var graph = GetGraph();

            Node dummy;
            GetNode(node1Id).Connections.TryRemove(node2Id, out dummy);
            GetNode(node2Id).Connections.TryRemove(node1Id, out dummy);

            Interlocked.Decrement(ref graph.ConnectionCount);

            _graphEventHandler.ConnectionDeleted(_graphDescriptor, node1Id, node2Id);
        }

        public IEnumerable<INodeToNodeConnector> GetAll(int skip, int count)
        {
            var addedConnections = new HashSet<string>();
            var connectors = new List<INodeToNodeConnector>();

            foreach (var node in GetGraph().Nodes.Values)
            {
                foreach (var connection in node.Connections)
                {
                    if (!addedConnections.Contains(node.Id + "-" + connection.Key) && !addedConnections.Contains(connection.Key + "-" + node.Id))
                    {
                        connectors.Add(new NodeConnector { Node1Id = node.Id, Node2Id = connection.Key });
                        addedConnections.Add(node.Id + "-" + connection.Key);
                    }
                }
            }

            return connectors.Cast<INodeToNodeConnector>();
        }

        public IEnumerable<int> GetNeighbourIds(int nodeId, int skip, int count)
        {
            return GetNode(nodeId).Connections.Keys;
        }

        public int GetNeighbourCount(int nodeId)
        {
            return GetNeighbourIds(nodeId, 0, int.MaxValue).Count();
        }


        protected Graph GetGraph()
        {
            return Storage.GetOrAdd(_graphDescriptor.Name, new Graph());
        }

        protected Node GetNode(int id)
        {
            return GetGraph().Nodes.GetOrAdd(id, new Node(id));
        }


        protected class Graph
        {
            public int ConnectionCount;
            public ConcurrentDictionary<int, Node> Nodes { get; private set; }

            public Graph()
            {
                ConnectionCount = 0;
                Nodes = new ConcurrentDictionary<int, Node>();
            }
        }

        protected class Node
        {
            public int Id { get; private set; }
            public ConcurrentDictionary<int, Node> Connections { get; private set; }

            public Node(int id)
            {
                Id = id;
                Connections = new ConcurrentDictionary<int, Node>();
            }
        }
    }
}