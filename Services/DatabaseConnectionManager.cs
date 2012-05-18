using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.GraphDiscovery;
using System.Collections.Concurrent;
using System;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes, persisting connections in the database
    /// </summary>
    /// <remarks>
    /// The memcaching is done with a static dictionary. Now this is fine if the site is run on a single server but will 
    /// eventually cause users to observe inconsistencies (as the memcache won't be updated if the DB is updated from a different
    /// isntance); DB will remain consistent though.
    /// Currently there is no real way to support a web farm scenario, see: http://orchard.codeplex.com/workitem/17361
    /// However, this will work just as in a single-server environment with proper cloud hosting like Gearhost.
    /// </remarks>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    [OrchardFeature("Associativy")]    
    public class DatabaseConnectionManager<TNodeToNodeConnectorRecord> : IDatabaseConnectionManager<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly IContentManager _contentManager;
        protected readonly IGraphEventHandler _graphEventHandler;

        // The inner dictionary should really be a concurrent HashSet
        // Race conditions could occur, revise if necessary.
        protected static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, int>> _connections = new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>();


        public DatabaseConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            IContentManager contentManager,
            IGraphEventHandler graphEventHandler)
        {
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _contentManager = contentManager;
            _graphEventHandler = graphEventHandler;

            // Although this happens once in the shell life time, should be called lazily or hooked into events, e.g. IOrchardShellEvents
            TryLoadConnections();
        }


        public void TryLoadConnections()
        {
            if (_connections.IsEmpty)
            {
                ReloadConnections();
            }
        }

        public void ReloadConnections()
        {
            _connections.Clear();

            // This apparently uses ~75KB memory with the test set of 80 connections.
            foreach (var connector in _nodeToNodeRecordRepository.Table)
            {
                StoreMemoryConnection(connector.Id, connector.Node1Id, connector.Node2Id);
            }
        }


        public virtual bool AreNeighbours(IGraphContext graphContext, int node1Id, int node2Id)
        {
            ConcurrentDictionary<int, int> subDictionary;

            if (_connections.TryGetValue(node1Id, out subDictionary))
            {
                return subDictionary.ContainsKey(node2Id);
            }

            return false;

            //return _nodeToNodeRecordRepository.Count(connector =>
            //    connector.Node1Id == node1Id && connector.Node2Id == node2Id ||
            //    connector.Node1Id == node2Id && connector.Node2Id == node1Id) != 0;
        }


        public virtual void Connect(IGraphContext graphContext, int node1Id, int node2Id)
        {
            if (!AreNeighbours(graphContext, node1Id, node2Id))
            {
                var connector = new TNodeToNodeConnectorRecord() { Node1Id = node1Id, Node2Id = node2Id };
                _nodeToNodeRecordRepository.Create(connector);
                StoreMemoryConnection(connector.Id, node1Id, node2Id);
                _graphEventHandler.ConnectionAdded(graphContext, node1Id, node2Id);
            }
        }


        public virtual void DeleteFromNode(IGraphContext graphContext, int nodeId)
        {
            // Since there is no cummulative delete...
            var connectionsToBeDeleted = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Node1Id == nodeId || connector.Node2Id == nodeId).ToList();

            foreach (var connector in connectionsToBeDeleted)
            {
                _nodeToNodeRecordRepository.Delete(connector);
            }

            ConcurrentDictionary<int, int> subDictionary;
            if (_connections.TryRemove(nodeId, out subDictionary))
            {
                int currentId;
                foreach (var neighbourId in subDictionary.Keys)
                {
                    _connections[neighbourId].TryRemove(neighbourId, out currentId);
                }
            }

            _graphEventHandler.ConnectionsDeletedFromNode(graphContext, nodeId);
        }

        public virtual void Disconnect(IGraphContext graphContext, int node1Id, int node2Id)
        {
            var connectorRecord = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Node1Id == node1Id && connector.Node2Id == node2Id ||
                connector.Node1Id == node2Id && connector.Node2Id == node1Id).FirstOrDefault();

            if (connectorRecord == null) return;

            _nodeToNodeRecordRepository.Delete(connectorRecord);

            int currentId;
            _connections[node1Id].TryRemove(node2Id, out currentId);
            _connections[node2Id].TryRemove(node1Id, out currentId);

            _graphEventHandler.ConnectionDeleted(graphContext, node1Id, node2Id);
        }


        public virtual IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext)
        {
            var records = new List<TNodeToNodeConnectorRecord>();

            foreach (var connections in _connections)
            {
                foreach (var connection in connections.Value)
                {
                    records.Add(new TNodeToNodeConnectorRecord { Id = connection.Value, Node1Id = connections.Key, Node2Id = connection.Key });
                }
            }

            return records.Cast<INodeToNodeConnector>();

            // Without .ToList(), "No persister for: Associativy.Models.INodeToNodeConnector" is thrown
            //return _nodeToNodeRecordRepository.Table.ToList().Select(r => (INodeToNodeConnector)r);
        }


        public virtual IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId)
        {
            ConcurrentDictionary<int, int> subDictionary;

            if (_connections.TryGetValue(nodeId, out subDictionary)) return subDictionary.Keys;

            return Enumerable.Empty<int>();

            //// Measure performance with large datasets, as .AsParallel() queries tend to be slower
            //return _nodeToNodeRecordRepository.
            //    Fetch(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId).
            //    Select(connector => connector.Node1Id == nodeId ? connector.Node2Id : connector.Node1Id);
        }

        public virtual int GetNeighbourCount(IGraphContext graphContext, int nodeId)
        {
            return GetNeighbourIds(graphContext, nodeId).Count();
            //return _nodeToNodeRecordRepository.
            //    Count(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId);
        }


        private static void StoreMemoryConnection(int id, int node1Id, int node2Id)
        {
            Action<int, int> storeConnection =
                (id1, id2) =>
                {
                    if (!_connections.ContainsKey(id1))
                    {
                        _connections[id1] = new ConcurrentDictionary<int, int>();
                    }

                    _connections[id1][id2] = id;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id);
            storeConnection(node2Id, node1Id);
        }
    }
}