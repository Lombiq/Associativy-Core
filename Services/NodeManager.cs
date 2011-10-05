using System;
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
    [OrchardFeature("Associativy")]
    public class NodeManager<TNodePart, TNodePartRecord> : INodeManager<TNodePart, TNodePartRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
    {
        protected readonly IContentManager _contentManager;
        protected readonly IRepository<TNodePartRecord> _nodePartRecordRepository;

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

        #region Node CRUD
        public IContentQuery<TNodePart, TNodePartRecord> ContentQuery
        {
            get { return _contentManager.Query<TNodePart, TNodePartRecord>(); }
        }

        public TNodePart Create(INodeParams<TNodePart> nodeParams)
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

        public TNodePart Get(string label)
        {
            // inkább LIKE-kal?
            return ContentQuery.Where(node => node.Label == label).List().FirstOrDefault();
        }

        public IList<TNodePart> GetMany(IList<int> ids)
        {
            return ContentQuery.Where(node => ids.Contains(node.Id)).List().ToList();
        }

        public TNodePart Update(INodeParams<TNodePart> nodeParams)
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