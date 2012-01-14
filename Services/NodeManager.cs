using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Routable.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class NodeManager : AssociativyServiceBase, INodeManager
    {
        protected readonly IContentManager _contentManager;
        protected readonly IAssociativeGraphEventHandler _graphEventHandler;

        public NodeManager(
            IContentManager contentManager,
            IAssociativyGraphDescriptor associativyGraphDescriptor,
            IAssociativeGraphEventHandler graphEventHandler)
            : base(associativyGraphDescriptor)
        {
            _contentManager = contentManager;
            _graphEventHandler = graphEventHandler;
        }

        public IContentQuery<ContentItem> ContentQuery
        {
            get { return _contentManager.Query(GraphDescriptor.ContentTypes); }
        }

        public IContentQuery<ContentItem> GetManyQuery(IEnumerable<int> ids)
        {
            // Otherwise an exception with message "Expression argument must be of type ICollection." is thrown Orchard.ContentManagement.DefaultContentQuery on line 90.
            var idsCollection = ids.ToList();
            return ContentQuery.Where<CommonPartRecord>(r => idsCollection.Contains(r.Id));
        }

        public virtual IEnumerable<IContent> GetSimilarNodes(string labelSnippet, int maxCount = 10, QueryHints queryHints = null)
        {
            if (String.IsNullOrEmpty(labelSnippet)) return null; // Otherwise would return the whole dataset
            if (queryHints == null) queryHints = new QueryHints();
            labelSnippet = labelSnippet.ToLowerInvariant();
            return ContentQuery.Where<AssociativyNodeLabelPartRecord>(r => r.InvariantLabel.StartsWith(labelSnippet)).WithQueryHints(queryHints).Slice(maxCount).ToList();
        }

        public virtual IContent Get(string label, QueryHints queryHints = null)
        {
            if (queryHints == null) queryHints = new QueryHints();
            label = label.ToLowerInvariant();
            return ContentQuery.Where<AssociativyNodeLabelPartRecord>(r => r.InvariantLabel == label).WithQueryHints(queryHints).List().FirstOrDefault();
        }

        public virtual IEnumerable<IContent> GetMany(IEnumerable<string> labels, QueryHints queryHints = null)
        {
            if (queryHints == null) queryHints = new QueryHints();

            var labelsArray = labels.ToArray();
            for (int i = 0; i < labelsArray.Length; i++)
            {
                labelsArray[i] = labelsArray[i].ToLowerInvariant();
            }

            return ContentQuery.Where<AssociativyNodeLabelPartRecord>(r => labelsArray.Contains(r.InvariantLabel)).WithQueryHints(queryHints).List();
        }
    }
}