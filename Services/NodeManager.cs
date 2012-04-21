using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class NodeManager : AssociativyServiceBase, INodeManager
    {
        protected readonly IContentManager _contentManager;
        protected readonly IGraphEventHandler _graphEventHandler;

        public NodeManager(
            IGraphManager graphManager,
            IContentManager contentManager,
            IGraphEventHandler graphEventHandler)
            : base(graphManager)
        {
            _contentManager = contentManager;
            _graphEventHandler = graphEventHandler;
        }

        public IContentQuery<ContentItem> GetContentQuery(IGraphContext graphContext)
        {
            return _contentManager.Query(_graphManager.FindGraph(graphContext).ContentTypes.ToArray());
        }

        public IContentQuery<ContentItem> GetManyContentQuery(IGraphContext graphContext, IEnumerable<int> ids)
        {
            // Otherwise an exception with message "Expression argument must be of type ICollection." is thrown from
            // Orchard.ContentManagement.DefaultContentQuery on line 90.
            var idsCollection = ids.ToList();
            return GetContentQuery(graphContext).Where<CommonPartRecord>(r => idsCollection.Contains(r.Id));
        }

        public virtual IEnumerable<IContent> GetSimilarNodes(IGraphContext graphContext, string labelSnippet, int maxCount = 10, QueryHints queryHints = null)
        {
            if (String.IsNullOrEmpty(labelSnippet)) return null; // Otherwise would return the whole dataset
            if (queryHints == null) queryHints = new QueryHints();
            labelSnippet = labelSnippet.ToUpperInvariant();
            return GetContentQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel.StartsWith(labelSnippet)).WithQueryHints(queryHints).Slice(maxCount).ToList();
        }

        public virtual IContent Get(IGraphContext graphContext, string label, QueryHints queryHints = null)
        {
            if (queryHints == null) queryHints = new QueryHints();
            label = label.ToUpperInvariant();
            return GetContentQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel == label).WithQueryHints(queryHints).List().FirstOrDefault();
        }

        public virtual IEnumerable<IContent> GetMany(IGraphContext graphContext, IEnumerable<string> labels, QueryHints queryHints = null)
        {
            if (queryHints == null) queryHints = new QueryHints();

            var labelsArray = labels.ToArray();
            for (int i = 0; i < labelsArray.Length; i++)
            {
                labelsArray[i] = labelsArray[i].ToUpperInvariant();
            }

            return GetContentQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => labelsArray.Contains(r.UpperInvariantLabel)).WithQueryHints(queryHints).List();
        }
    }
}