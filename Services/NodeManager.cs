using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class NodeManager : AssociativyServiceBase, INodeManager
    {
        protected readonly IContentManager _contentManager;
        protected readonly INodeManagerEventHander _eventHandler;


        public NodeManager(
            IGraphManager graphManager,
            IContentManager contentManager,
            INodeManagerEventHander eventHandler)
            : base(graphManager)
        {
            _contentManager = contentManager;
            _eventHandler = eventHandler;
        }


        public IContentQuery<ContentItem> GetQuery(IGraphContext graphContext)
        {
            var query = _contentManager.Query(_graphManager.FindGraph(graphContext).ContentTypes.ToArray());
            _eventHandler.QueryBuilt(new QueryBuiltContext(graphContext, query));
            return query;
        }

        public IContentQuery<ContentItem> GetManyQuery(IGraphContext graphContext, IEnumerable<int> ids)
        {
            // Otherwise an exception with message "Expression argument must be of type ICollection." is thrown from
            // Orchard.ContentManagement.DefaultContentQuery on line 90.
            var idsList = ids.ToList();
            return GetQuery(graphContext).Where<CommonPartRecord>(r => idsList.Contains(r.Id));
        }

        public virtual IContentQuery<ContentItem> GetSimilarNodesQuery(IGraphContext graphContext, string labelSnippet)
        {
            labelSnippet = labelSnippet.ToUpperInvariant();
            return GetQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel.StartsWith(labelSnippet));
        }


        public virtual IContentQuery<ContentItem> GetManySimilarNodesQuery(IGraphContext graphContext, IEnumerable<string> labels)
        {
            var labelsArray = labels.ToArray();
            for (int i = 0; i < labelsArray.Length; i++)
            {
                labelsArray[i] = labelsArray[i].ToUpperInvariant();
            }

            return GetQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => labelsArray.Contains(r.UpperInvariantLabel));
        }

        public virtual IContentQuery<ContentItem> GetByLabelQuery(IGraphContext graphContext, string label)
        {
            label = label.ToUpperInvariant();
            return GetQuery(graphContext).Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel == label);
        }
    }
}