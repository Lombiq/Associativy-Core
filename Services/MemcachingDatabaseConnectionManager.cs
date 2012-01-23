using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Models;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Orchard.Data;
using Associativy.EventHandlers;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class MemcachingDatabaseConnectionManager<TNodeToNodeConnectorRecord> :
        DatabaseConnectionManager<TNodeToNodeConnectorRecord>, IMemcachingDatabaseConnectionManager<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        // The inner dictionary should really be a concurrent HashSet
        // Race conditionas could occure, revise if necessary.
        protected static ConcurrentDictionary<int, ConcurrentDictionary<int, object>> _connections = new ConcurrentDictionary<int, ConcurrentDictionary<int, object>>();

        public MemcachingDatabaseConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            IContentManager contentManager,
            IGraphEventHandler graphEventHandler)
            : base(nodeToNodeRecordRepository, contentManager, graphEventHandler)
        {
            // Although this happens once in the shell life time, should be called lazily or hooked into events, e.g. IOrchardShellEvents
            LoadConnections();
        }


        public void LoadConnections()
        {
            if (_connections.IsEmpty)
            {
                // This apparently uses ~75KB memory with the test set of 80 connections.
                foreach (var connector in _nodeToNodeRecordRepository.Table)
                {
                    StoreMemoryConnection(connector.Node1Id, connector.Node2Id);
                } 
            }
        }


        public override bool AreNeighbours(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            ConcurrentDictionary<int, object> subDictionary;

            if (_connections.TryGetValue(nodeId1, out subDictionary))
            {
                return subDictionary.ContainsKey(nodeId2);
            }

            return false;
        }


        public override void Connect(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            StoreMemoryConnection(nodeId1, nodeId2);

            base.Connect(graphContext, nodeId1, nodeId2);
        }


        public override void DeleteFromNode(IGraphContext graphContext, int nodeId)
        {
            // No check here, could be a problem...
            ConcurrentDictionary<int, object> subDictionary;
            if (_connections.TryRemove(nodeId, out subDictionary))
            {
                object currentObject;
                foreach (var neighbourId in subDictionary.Keys)
                {
                    _connections[neighbourId].TryRemove(neighbourId, out currentObject);
                }
            }

            base.DeleteFromNode(graphContext, nodeId);
        }

        public override void Disconnect(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            object currentObject;

            _connections[nodeId1].TryRemove(nodeId2, out currentObject);
            _connections[nodeId2].TryRemove(nodeId1, out currentObject);

            base.Disconnect(graphContext, nodeId1, nodeId2);
        }


        public override IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext)
        {
            return base.GetAll(graphContext);
        }

        public override IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId)
        {
            return _connections[nodeId].Keys;
        }


        protected void StoreMemoryConnection(int node1Id, int node2Id)
        {
            Action<int, int> storeConnection =
                (id1, id2) =>
                {
                    if (!_connections.ContainsKey(id1))
                    {
                        _connections[id1] = new ConcurrentDictionary<int, object>();
                    }

                    _connections[id1][id2] = null;
                };

            // Storing both ways for fast access.
            storeConnection(node1Id, node2Id);
            storeConnection(node2Id, node1Id);
        }
    }
}