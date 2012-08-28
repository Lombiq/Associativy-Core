﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Connections manager storing the graph in memory (i.e. not persisting it)
    /// </summary>
    public class MemoryConnectionManager : AssociativyServiceBase, IMemoryConnectionManager
    {
        protected readonly IMemoryConnectionStorage _connectionStorage;
        protected readonly IGraphEventHandler _graphEventHandler;

        public MemoryConnectionManager(
            IGraphManager graphManager,
            IMemoryConnectionStorage connectionStorage,
            IGraphEventHandler graphEventHandler)
            : base(graphManager)
        {
            _connectionStorage = connectionStorage;
            _graphEventHandler = graphEventHandler;
        }

        public int GetConnectionCount(IGraphContext graphContext)
        {
            if (!_connectionStorage.Connections.ContainsKey(graphContext.GraphName)) return 0;
            // Dividing by 2 since connections are stored both ways
            return _connectionStorage.Connections[graphContext.GraphName].Count / 2;
        }

        public bool AreNeighbours(IGraphContext graphContext, int node1Id, int node2Id)
        {
            ConcurrentDictionary<int, int> subDictionary;

            if (GetGraphConnections(graphContext).TryGetValue(node1Id, out subDictionary))
            {
                return subDictionary.ContainsKey(node2Id);
            }

            return false;
        }

        public void Connect(IGraphContext graphContext, int node1Id, int node2Id)
        {
            Connect(graphContext, _connectionStorage.Connections.Count, node1Id, node2Id);
        }

        public void Connect(IGraphContext graphContext, int connectionId, int node1Id, int node2Id)
        {
            if (AreNeighbours(graphContext, node1Id, node2Id)) return;

            Action<int, int> storeConnection =
                (id1, id2) =>
                {
                    var subDictionary = GetGraphConnections(graphContext).GetOrAdd(id1, new ConcurrentDictionary<int, int>());
                    subDictionary[id2] = connectionId;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id);
            storeConnection(node2Id, node1Id);

            _graphEventHandler.ConnectionAdded(graphContext, node1Id, node2Id);
        }

        public void DeleteFromNode(IGraphContext graphContext, int nodeId)
        {
            var graphConnections = GetGraphConnections(graphContext);
            ConcurrentDictionary<int, int> subDictionary;
            if (graphConnections.TryRemove(nodeId, out subDictionary))
            {
                int currentId;
                foreach (var neighbourId in subDictionary.Keys)
                {
                    graphConnections[neighbourId].TryRemove(neighbourId, out currentId);
                }
            }

            _graphEventHandler.ConnectionsDeletedFromNode(graphContext, nodeId);
        }

        public void Disconnect(IGraphContext graphContext, int node1Id, int node2Id)
        {
            if (!AreNeighbours(graphContext, node1Id, node2Id)) return;

            var graphConnections = GetGraphConnections(graphContext);
            int currentId;
            graphConnections[node1Id].TryRemove(node2Id, out currentId);
            graphConnections[node2Id].TryRemove(node1Id, out currentId);

            _graphEventHandler.ConnectionDeleted(graphContext, node1Id, node2Id);
        }

        public IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext)
        {
            var records = new List<INodeToNodeConnector>();

            foreach (var connections in GetGraphConnections(graphContext))
            {
                foreach (var connection in connections.Value)
                {
                    records.Add(new NodeConnector { Id = connection.Value, Node1Id = connections.Key, Node2Id = connection.Key });
                }
            }

            return records.Cast<INodeToNodeConnector>();
        }

        public IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId)
        {
            ConcurrentDictionary<int, int> subDictionary;

            if (GetGraphConnections(graphContext).TryGetValue(nodeId, out subDictionary)) return subDictionary.Keys;

            return Enumerable.Empty<int>();
        }

        public int GetNeighbourCount(IGraphContext graphContext, int nodeId)
        {
            return GetNeighbourIds(graphContext, nodeId).Count();
        }

        private string GetGraphName(IGraphContext graphContext)
        {
            return _graphManager.FindGraph(graphContext).GraphName;
        }

        private ConcurrentDictionary<int, ConcurrentDictionary<int, int>> GetGraphConnections(IGraphContext graphContext)
        {
            return _connectionStorage.Connections.GetOrAdd(_graphManager.FindGraph(graphContext).GraphName, new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>());
        }

        private class NodeConnector : INodeToNodeConnector
        {
            public int Id { get; set; }
            public int Node1Id { get; set; }
            public int Node2Id { get; set; }
        }
    }


    public interface IMemoryConnectionStorage : ISingletonDependency
    {
        ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, int>>> Connections { get; }
    }

    /// <summary>
    /// Singleton storage for MemoryConnectionManager
    /// </summary>
    /// <remarks>
    /// This is separated from the connection manager so dependency injection can't be mixed up (a singleton MemoryConnectionManager could use Work<>,
    /// but this is easier).
    /// 
    /// In-memory storage is done with a dictionary. Now this is fine if the site is run on a single server but will 
    /// eventually cause users to observe inconsistencies (as the storage won't be updated if the DB is updated from a different
    /// isntance) on multi-server installations (DB will remain consistent though, if this is used in conjunction with DatabaseConnectionManager).
    /// Currently there is no real way to support a web farm scenario, see: http://orchard.codeplex.com/workitem/17361
    /// However, this will work just as in a single-server environment with proper cloud hosting like Gearhost.
    /// </remarks>
    public class MemoryConnectionStorage : IMemoryConnectionStorage
    {
        /// <summary>
        /// Stores the connections.
        /// Schema: [graphName][node1Id][node2Id] = connectionId. This aims fast lookup of connections between two nodes.
        /// </summary>
        /// <remarks>
        /// Race conditions could occur, revise if necessary.
        /// This apparently uses ~75KB memory with a test set of 80 connections.
        /// </remarks>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, int>>> _connections = new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, int>>>();
        public ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, int>>> Connections
        {
            get { return _connections; }
        }
    }
}