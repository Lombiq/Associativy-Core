using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard.Caching;
using Associativy.Models.Nodes;

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
        /// Schema: [graphName][node1Id][node2Id] = connectionId. This aims fast lookup of connections between two nodes.
        /// </summary>
        /// <remarks>
        /// Race conditions could occur, revise if necessary.
        /// This apparently uses ~75KB of memory with a test set of 80 connections.
        /// </remarks>
        protected ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>> Connections
        {
            get
            {
                return _cacheManager.Get("Associativy.GraphStorage", ctx =>
                {
                    return new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>>();
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
            if (!Connections.ContainsKey(_graphDescriptor.Name)) return 0;
            // Dividing by 2 since connections are stored both ways
            return Connections[_graphDescriptor.Name].Count / 2;
        }

        public bool AreNeighbours(int node1Id, int node2Id)
        {
            if (node1Id == node2Id) return true;

            ConcurrentDictionary<int, bool> subDictionary;

            if (GetGraphConnections().TryGetValue(node1Id, out subDictionary))
            {
                return subDictionary.ContainsKey(node2Id);
            } 

            return false;
        }

        public void Connect(int node1Id, int node2Id)
        {
            if (AreNeighbours(node1Id, node2Id)) return;

            Action<int, int> storeConnection =
                (id1, id2) =>
                {
                    var subDictionary = GetGraphConnections().GetOrAdd(id1, new ConcurrentDictionary<int, bool>());
                    subDictionary[id2] = true;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id);
            storeConnection(node2Id, node1Id);

            _graphEventHandler.ConnectionAdded(_graphDescriptor, node1Id, node2Id);
        }

        public void DeleteFromNode(int nodeId)
        {
            var graphConnections = GetGraphConnections();
            ConcurrentDictionary<int, bool> subDictionary;
            if (graphConnections.TryRemove(nodeId, out subDictionary))
            {
                bool dummyValue;
                foreach (var neighbourId in subDictionary.Keys)
                {
                    graphConnections[neighbourId].TryRemove(neighbourId, out dummyValue);
                }
            }

            _graphEventHandler.ConnectionsDeletedFromNode(_graphDescriptor, nodeId);
        }

        public void Disconnect(int node1Id, int node2Id)
        {
            if (!AreNeighbours(node1Id, node2Id)) return;

            var graphConnections = GetGraphConnections();
            bool dummyValue;
            graphConnections[node1Id].TryRemove(node2Id, out dummyValue);
            graphConnections[node2Id].TryRemove(node1Id, out dummyValue);

            _graphEventHandler.ConnectionDeleted(_graphDescriptor, node1Id, node2Id);
        }

        public IEnumerable<INodeToNodeConnector> GetAll()
        {
            var addedConnections = new HashSet<string>();
            var connectors = new List<INodeToNodeConnector>();

            foreach (var connections in GetGraphConnections())
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

        public IEnumerable<int> GetNeighbourIds(int nodeId)
        {
            ConcurrentDictionary<int, bool> subDictionary;

            if (GetGraphConnections().TryGetValue(nodeId, out subDictionary)) return subDictionary.Keys;

            return Enumerable.Empty<int>();
        }

        public int GetNeighbourCount(int nodeId)
        {
            return GetNeighbourIds(nodeId).Count();
        }


        private ConcurrentDictionary<int, ConcurrentDictionary<int, bool>> GetGraphConnections()
        {
            return Connections.GetOrAdd(_graphDescriptor.Name, new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>());
        }
    }
}