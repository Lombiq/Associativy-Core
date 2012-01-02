using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.EventHandlers;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class ConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> 
        : AssociativyService<TAssociativyContext>, IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly IContentManager _contentManager;
        protected readonly IAssociativeGraphEventHandler _associativeGraphEventHandler;

        public ConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            TAssociativyContext associativyContext,
            IContentManager contentManager,
            IAssociativeGraphEventHandler associativeGraphEventHandler)
            : base(associativyContext)
        {
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _contentManager = contentManager;
            _associativeGraphEventHandler = associativeGraphEventHandler;
        }

        public virtual bool AreNeighbours(int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Node1Id == nodeId1 && connector.Node2Id == nodeId2 ||
                connector.Node1Id == nodeId2 && connector.Node2Id == nodeId1) != 0;
        }

        public virtual void Connect(IContent node1, IContent node2)
        {
            Connect(node1.Id, node2.Id);
        }

        public virtual void Connect(int nodeId1, int nodeId2)
        {
            if (_contentManager.Get(nodeId1) == null || _contentManager.Get(nodeId2) == null) return; // No such nodes exist

            if (!AreNeighbours(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Node1Id = nodeId1, Node2Id = nodeId2 });
            }

            _associativeGraphEventHandler.Changed(_associativyContext);
        }

        public virtual void DeleteFromNode(IContent node)
        {
            DeleteFromNode(node.Id);
        }

        public virtual void DeleteFromNode(int nodeId)
        {
            // Since there is no cummulative delete...
            var connectionsToBeDeleted = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Node1Id == nodeId || connector.Node2Id == nodeId).ToList();

            foreach (var connector in connectionsToBeDeleted)
            {
                _nodeToNodeRecordRepository.Delete(connector);
            }

            _associativeGraphEventHandler.Changed(_associativyContext);
        }

        public virtual void Delete(int id)
        {
            _nodeToNodeRecordRepository.Delete(_nodeToNodeRecordRepository.Get(id));

            _associativeGraphEventHandler.Changed(_associativyContext);
        }

        public virtual IEnumerable<TNodeToNodeConnectorRecord> GetAll()
        {
            return _nodeToNodeRecordRepository.Table;
        }

        public virtual IEnumerable<int> GetNeighbourIds(int nodeId)
        {
            // Measure performance with large datasets, as .AsParallel() queries tend to be slower
            return _nodeToNodeRecordRepository.
                Fetch(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId).
                Select(connector => connector.Node1Id == nodeId ? connector.Node2Id : connector.Node1Id);
        }

        public virtual int GetNeighbourCount(int nodeId)
        {
            return _nodeToNodeRecordRepository.
                Count(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId);
        }
    }
}