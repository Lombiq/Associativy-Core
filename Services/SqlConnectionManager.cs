﻿using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Associativy.Models.Services;
using Orchard.Data;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes, persisting connections in the SQL database
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public class SqlConnectionManager<TNodeToNodeConnectorRecord> : SqlConnectionManagerBase<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        public SqlConnectionManager(
            IGraphDescriptor graphDescriptor,
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            Func<IGraphDescriptor, IMemoryConnectionManager> memoryConnectionManagerFactory,
            IGraphEventHandler graphEventHandler)
            : base(graphDescriptor, nodeToNodeRecordRepository, memoryConnectionManagerFactory, graphEventHandler)
        {
        }


        public override void TryLoadConnections()
        {
            if (_memoryConnectionManager.GetConnectionCount() != 0) return;

            foreach (var connector in _nodeToNodeRecordRepository.Table)
            {
                _memoryConnectionManager.Connect(connector.Node1Id, connector.Node2Id);
            }
        }


        public override void Connect(int node1Id, int node2Id)
        {
            TryLoadConnections();

            if (AreNeighbours(node1Id, node2Id)) return;

            var connector = new TNodeToNodeConnectorRecord() { Node1Id = node1Id, Node2Id = node2Id };
            _nodeToNodeRecordRepository.Create(connector);
            _memoryConnectionManager.Connect(node1Id, node2Id);
            _graphEventHandler.ConnectionAdded(_graphDescriptor, node1Id, node2Id);
        }

        public override void DeleteFromNode(int nodeId)
        {
            TryLoadConnections();

            // Since there is no cummulative delete...
            var connectionsToBeDeleted = _nodeToNodeRecordRepository.Fetch(
                    connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId);

            foreach (var connector in connectionsToBeDeleted)
            {
                _nodeToNodeRecordRepository.Delete(connector);
            }

            _memoryConnectionManager.DeleteFromNode(nodeId);

            _graphEventHandler.ConnectionsDeletedFromNode(_graphDescriptor, nodeId);
        }

        public override void Disconnect(int node1Id, int node2Id)
        {
            TryLoadConnections();

            var connectorRecord = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Node1Id == node1Id && connector.Node2Id == node2Id ||
                connector.Node1Id == node2Id && connector.Node2Id == node1Id).FirstOrDefault();

            if (connectorRecord == null) return;

            _nodeToNodeRecordRepository.Delete(connectorRecord);

            _memoryConnectionManager.Disconnect(node1Id, node2Id);

            _graphEventHandler.ConnectionDeleted(_graphDescriptor, node1Id, node2Id);
        }
    }
}