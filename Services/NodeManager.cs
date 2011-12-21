using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Events;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class NodeManager<TNodePart, TNodePartRecord> : INodeManager<TNodePart, TNodePartRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
    {
        protected readonly IContentManager _contentManager;
        protected readonly IRepository<TNodePartRecord> _nodePartRecordRepository;

        public event EventHandler<GraphChangedEventArgs> GraphChanged;

        public NodeManager(
            IContentManager contentManager,
            IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            _contentManager = contentManager;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        public virtual IList<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return _nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).Take(maxCount).ToList();
        }

        #region Node CRUD
        public IContentQuery<TNodePart, TNodePartRecord> ContentQuery
        {
            get { return _contentManager.Query<TNodePart, TNodePartRecord>(); }
        }

        public virtual TNodePart Create(INodeParams<TNodePart> nodeParams)
        {
            var node = _contentManager.New<TNodePart>(nodeParams.ContentTypeName);
            nodeParams.MapToNode(node);
            _contentManager.Create(node);

            OnGraphChanged();

            return node;
        }

        public virtual TNodePart New(string contentType)
        {
            return _contentManager.New<TNodePart>(contentType);
        }

        public virtual void Create(ContentItem node)
        {
            _contentManager.Create(node);
            OnGraphChanged();
        }

        public virtual TNodePart Get(int id)
        {
            return _contentManager.Get<TNodePart>(id);
        }

        public virtual TNodePart Get(string label)
        {
            // Maybe rather as something like with a LIKE query?
            return ContentQuery.Where(node => node.Label == label).List().FirstOrDefault();
        }

        public virtual IList<TNodePart> GetMany(IList<int> ids)
        {
            //? contentManager.GetMany<TNodePart>(ids, VersionOptions.AllVersions, new QueryHints().ExpandParts<TNodePart>());
            return ContentQuery.Where(node => ids.Contains(node.Id)).List().ToList();
        }

        public virtual TNodePart Update(INodeParams<TNodePart> nodeParams)
        {
            if (nodeParams.Id == 0) throw new ArgumentException("When updating a node the Id property of the INodeParams object should be set.");

            var node = Get(nodeParams.Id);
            if (node != null)
            {
                nodeParams.MapToNode(node);
                _contentManager.Flush();

                OnGraphChanged();
            }

            return node;
        }

        public virtual TNodePart Update(TNodePart node)
        {
            // What should happen with other parts?
            if (node.Id == 0) throw new ArgumentException("When updating a node the Id property of the INode object should be set. (Maybe you tried to update a new, not yet created part?)");

            _contentManager.Flush();

            OnGraphChanged();

            return node;
        }

        public virtual void Remove(int id)
        {
            _contentManager.Remove(_contentManager.Get(id));

            OnGraphChanged();
        }
        #endregion

        private void OnGraphChanged()
        {
            if (GraphChanged != null)
            {
                GraphChanged(this, new GraphChangedEventArgs()); 
            }
        }
    }
}