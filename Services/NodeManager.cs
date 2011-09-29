using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using Orchard.Data;

namespace Associativy.Services
{
    public class NodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly IContentManager _contentManager;
        private readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        private readonly IRepository<TNodePartRecord> _nodePartRecordRepository;

        public NodeManager(
            IContentManager contentManager,
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            _contentManager = contentManager;
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        #region Connection management
        public bool AreConnected(int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Record1Id == nodeId1 && connector.Record2Id == nodeId2 ||
                connector.Record1Id == nodeId2 && connector.Record2Id == nodeId1) != 0;
        }

        public void AddConnection(TNodePart node1, TNodePart node2)
        {
            AddConnection(node1.Id, node2.Id);
        }

        public void AddConnection(int nodeId1, int nodeId2)
        {
            if (Get(nodeId1) == null || Get(nodeId2) == null) return; // No such nodes

            if (!AreConnected(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = nodeId1, Record2Id = nodeId2 });
            }
        }

        public IList<TNodeToNodeConnectorRecord> GetAllConnections()
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
        #endregion

        public IList<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return _nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).Take(maxCount).ToList();
        }

        #region Node CRUD
        public IContentQuery<TNodePart, TNodePartRecord> ContentQuery
        {
            get { return _contentManager.Query<TNodePart, TNodePartRecord>(); }
        }

        public TNodePart Create(TNodeParams nodeParams)
        {
            var node = _contentManager.New<TNodePart>(nodeParams.ContentTypeName);
            nodeParams.MapToPart(node);
            _contentManager.Create(node);

            return node;
        }

        public TNodePart Get(int id)
        {
            return _contentManager.Get<TNodePart>(id);
        }

        public TNodePart Update(TNodeParams nodeParams)
        {
            if (nodeParams.Id == 0) throw new ArgumentException("When updating a node the Id property of the INodeParams object should be set.");

            var node = Get(nodeParams.Id);
            if (node != null)
            {
                nodeParams.MapToPart(node);
                _contentManager.Flush();
            }

            return node;
        }

        public TNodePart Update(TNodePart node)
        {
            if (node.Id == 0) throw new ArgumentException("When updating a node the Id property of the INode object should be set. (Maybe you tried to update a new, not yet created part?)");

            _contentManager.Flush();

            return node;
        }

        public void Delete(int id)
        {
            // Since there is no cummulative delete...
            var connectionsToBeDeleted = _nodeToNodeRecordRepository.Fetch(connector =>
                connector.Record1Id == id || connector.Record2Id == id).ToList();

            foreach (var connector in connectionsToBeDeleted)
            {
                _nodeToNodeRecordRepository.Delete(connector);
            }

            _contentManager.Remove(_contentManager.Get(id));
        }
        #endregion
    }
}