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
    public class NodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly IContentManager _contentManager;

        private readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        public IRepository<TNodeToNodeConnectorRecord> NodeToNodeRecordRepository
        {
            get { return _nodeToNodeRecordRepository; }
        }

        private readonly IRepository<TNodePartRecord> _nodePartRecordRepository;
        public IRepository<TNodePartRecord> NodePartRecordRepository
        {
            get { return _nodePartRecordRepository; }
        }

        public NodeManager(
            IContentManager contentManager,
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            _contentManager = contentManager;
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        public void AddConnection(TNodePart node1, TNodePart node2)
        {
            AddConnection(node1.Id, node2.Id);
        }

        public void AddConnection(int nodeId1, int nodeId2)
        {
            if (GetNode(nodeId1) == null || GetNode(nodeId2) == null) return; // No such nodes

            if (!AreConnected(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = nodeId1, Record2Id = nodeId2 });
            }
        }

        public List<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return _nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).Take(maxCount).ToList();
        }

        #region Node CRUD
        public TNodePart CreateNode<TNodeParams>(TNodeParams nodeParams) where TNodeParams : INodeParams<TNodePart>
        {
            var node = _contentManager.New<TNodePart>(nodeParams.ContentTypeName);
            nodeParams.MapToPart(node);
            _contentManager.Create(node);

            return node;
        }

        public TNodePart GetNode(int id)
        {
            return _contentManager.Get<TNodePart>(id);
        }

        //public TNodePart UpdateNode()
        //{
        //}

        public void DeleteNode(int id)
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

        // Maybe protected internal?
        public IList<int> GetNeighbourIds(int nodeId)
        {
            // Measure performance with large datasets, as .AsParallel() queries tend to be slower
            return _nodeToNodeRecordRepository.
                Fetch(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId).
                Select(connector => connector.Record1Id == nodeId ? connector.Record2Id : connector.Record1Id).ToList();
        }

        public bool AreConnected(int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Record1Id == nodeId1 && connector.Record2Id == nodeId2 ||
                connector.Record1Id == nodeId2 && connector.Record2Id == nodeId1) != 0;
        }

        public int GetNeighbourCount(int nodeId)
        {
            return _nodeToNodeRecordRepository.
                Count(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId);
        }

        public IEnumerable<TNodePart> GetSucceededNodes(List<List<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>();
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList());
            _contentManager.Query<TNodePart, TNodePartRecord>();
            return _contentManager.Query<TNodePart, TNodePartRecord>().Where(node => succeededNodeIds.Contains(node.Id)).List();
        }
    }
}