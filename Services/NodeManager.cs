using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.Events;

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
        protected readonly IContentManager contentManager;
        protected readonly IRepository<TNodePartRecord> nodePartRecordRepository;

        public event EventHandler<GraphEventArgs> GraphChanged;

        public NodeManager(
            IContentManager contentManager,
            IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            this.contentManager = contentManager;
            this.nodePartRecordRepository = nodePartRecordRepository;
        }

        public IList<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).Take(maxCount).ToList();
        }

        #region Node CRUD
        public IContentQuery<TNodePart, TNodePartRecord> ContentQuery
        {
            get { return contentManager.Query<TNodePart, TNodePartRecord>(); }
        }

        public TNodePart Create(INodeParams<TNodePart> nodeParams)
        {
            var node = contentManager.New<TNodePart>(nodeParams.ContentTypeName);
            nodeParams.MapToNode(node);
            contentManager.Create(node);

            OnGraphChanged();

            return node;
        }

        public TNodePart Get(int id)
        {
            return contentManager.Get<TNodePart>(id);
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
                nodeParams.MapToNode(node);
                contentManager.Flush();

                OnGraphChanged();
            }

            return node;
        }

        public TNodePart Update(TNodePart node)
        {
            if (node.Id == 0) throw new ArgumentException("When updating a node the Id property of the INode object should be set. (Maybe you tried to update a new, not yet created part?)");

            contentManager.Flush();

            OnGraphChanged();

            return node;
        }

        public void Remove(int id)
        {
            contentManager.Remove(contentManager.Get(id));

            OnGraphChanged();
        }
        #endregion

        private void OnGraphChanged()
        {
            if (GraphChanged != null)
            {
                GraphChanged(this, new GraphEventArgs()); 
            }
        }
    }
}