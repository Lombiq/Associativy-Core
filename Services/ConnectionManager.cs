using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.Events;
using System;

namespace Associativy.Services
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// All suitable methods protected to aid inheritence.
    /// </remarks>
    /// <typeparam name="TNodePart"></typeparam>
    /// <typeparam name="TNodePartRecord"></typeparam>
    [OrchardFeature("Associativy")]
    // Önmagában nem kellene a TNodePartRecord és a TNodePart is csak az Add-nél
    public class ConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;

        public event EventHandler<GraphEventArgs> GraphChanged;

        public ConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            INodeManager<TNodePart, TNodePartRecord> nodeManager)
        {
            this._nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            this._nodeManager = nodeManager;
        }

        public bool AreNeighbours(int nodeId1, int nodeId2)
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

            if (!AreNeighbours(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = nodeId1, Record2Id = nodeId2 });
            }

            OnGraphChanged();
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

            OnGraphChanged();
        }

        public void Delete(int id)
        {
            _nodeToNodeRecordRepository.Delete(_nodeToNodeRecordRepository.Get(id));

            OnGraphChanged();
        }

        public IList<TNodeToNodeConnectorRecord> GetAll()
        {
            return _nodeToNodeRecordRepository.Table.ToList();
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

        // TODO: refactor to DRY (see NodeManager with the same)
        private void OnGraphChanged()
        {
            if (GraphChanged != null)
            {
                GraphChanged(this, new GraphEventArgs());
            }
        }
    }
}