using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class DatabaseConnectionManager<TNodeToNodeConnectorRecord> : IDatabaseConnectionManager<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly IContentManager _contentManager;
        protected readonly IGraphEventHandler _graphEventHandler;


        public DatabaseConnectionManager(
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            IContentManager contentManager,
            IGraphEventHandler graphEventHandler)
        {
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _contentManager = contentManager;
            _graphEventHandler = graphEventHandler;
        }


        public virtual bool AreNeighbours(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Node1Id == nodeId1 && connector.Node2Id == nodeId2 ||
                connector.Node1Id == nodeId2 && connector.Node2Id == nodeId1) != 0;
        }


        public virtual void Connect(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            if (!AreNeighbours(graphContext, nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Node1Id = nodeId1, Node2Id = nodeId2 });
            }

            _graphEventHandler.ConnectionAdded(graphContext, nodeId1, nodeId2);
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

            _graphEventHandler.ConnectionsDeletedFromNode(graphContext, nodeId);
        }

        public virtual void Disconnect(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            var connectorRecord = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Node1Id == nodeId1 && connector.Node2Id == nodeId2 ||
                connector.Node1Id == nodeId2 && connector.Node2Id == nodeId1).FirstOrDefault();

            if (connectorRecord == null) return;

            _nodeToNodeRecordRepository.Delete(connectorRecord);

            _graphEventHandler.ConnectionDeleted(graphContext, nodeId1, nodeId2);
        }


        public virtual IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext)
        {
            // Without .ToList(), "No persister for: Associativy.Models.INodeToNodeConnector" is thrown
            return _nodeToNodeRecordRepository.Table.ToList().Select(r => (INodeToNodeConnector)r);
        }


        public virtual IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId)
        {
            // Measure performance with large datasets, as .AsParallel() queries tend to be slower
            return _nodeToNodeRecordRepository.
                Fetch(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId).
                Select(connector => connector.Node1Id == nodeId ? connector.Node2Id : connector.Node1Id);
        }

        public virtual int GetNeighbourCount(IGraphContext graphContext, int nodeId)
        {
            return _nodeToNodeRecordRepository.
                Count(connector => connector.Node1Id == nodeId || connector.Node2Id == nodeId);
        }
    }
}