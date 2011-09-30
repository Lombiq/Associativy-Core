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
    public class NodeManager<TNodePart, TNodePartRecord, TNodeParams> : INodeManager<TNodePart, TNodePartRecord, TNodeParams>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
    {
        private readonly IContentManager _contentManager;
        private readonly IRepository<TNodePartRecord> _nodePartRecordRepository;

        public NodeManager(
            IContentManager contentManager,
            IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            _contentManager = contentManager;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        public IList<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return _nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).Take(maxCount).ToList();
        }

        public TNodeParams NodeParamsFactory()
        {
            return new TNodeParams();
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

        public void Remove(int id)
        {
            // delete connection

            _contentManager.Remove(_contentManager.Get(id));
        }
        #endregion
    }
}