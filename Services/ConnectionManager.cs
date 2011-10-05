﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    /// <summary>
    /// All suitable methods protected to aid inheritence.
    /// </summary>
    /// <typeparam name="TNodePart"></typeparam>
    /// <typeparam name="TNodePartRecord"></typeparam>
    /// <typeparam name="TNodeToNodeConnectorRecord"></typeparam>
    [OrchardFeature("Associativy")]
    public class ConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;

        public ConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            INodeManager<TNodePart, TNodePartRecord> nodeManager)
        {
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _nodeManager = nodeManager;
        }

        public bool AreConnected(int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Record1Id == nodeId1 && connector.Record2Id == nodeId2 ||
                connector.Record1Id == nodeId2 && connector.Record2Id == nodeId1) != 0;
        }

        public void Add(TNodePart node1, TNodePart node2)
        {
            Add(node1.Id, node2.Id);
        }

        public void Add(int nodeId1, int nodeId2)
        {
            if (_nodeManager.Get(nodeId1) == null || _nodeManager.Get(nodeId2) == null) return; // No such nodes

            if (!AreConnected(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = nodeId1, Record2Id = nodeId2 });
            }
        }

        public void DeleteMany(int nodeId)
        {
            // Since there is no cummulative delete...
            var connectionsToBeDeleted = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Record1Id == nodeId || connector.Record2Id == nodeId).ToList();

            foreach (var connector in connectionsToBeDeleted)
            {
                _nodeToNodeRecordRepository.Delete(connector);
            }
        }

        public void Delete(int id)
        {
            _nodeToNodeRecordRepository.Delete(_nodeToNodeRecordRepository.Get(id));
        }

        public IList<TNodeToNodeConnectorRecord> GetAll()
        {
            return _nodeToNodeRecordRepository.Table.ToList<TNodeToNodeConnectorRecord>();
        }

        public IList<int> GetNeighbourIds(int nodeId)
        {
            // Measure performance with large datasets, as .AsParallel() queries tend to be slower
            return _nodeToNodeRecordRepository.
                Fetch(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId).
                Select(connector => connector.Record1Id == nodeId ? connector.Record2Id : connector.Record1Id).ToList();
        }

        public int GetNeighbourCount(int nodeId)
        {
            return _nodeToNodeRecordRepository.
                Count(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId);
        }
    }
}