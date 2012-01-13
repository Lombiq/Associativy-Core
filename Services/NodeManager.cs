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

        // Better name?
        public virtual IEnumerable<string> GetSimilarTerms(string snippet, int maxCount = 10)
        {
            if (String.IsNullOrEmpty(snippet)) return null; // Otherwise would return the whole dataset
            return ContentQuery.Where<RoutePartRecord>(r => r.Title.StartsWith(snippet)).Slice(maxCount).ToList().Select(item => item.As<RoutePart>().Title);
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

        // Better name for label?
        public virtual IContent Get(string label)
        {
            // Maybe rather as something like with a LIKE query?
            return ContentQuery.Where<RoutePartRecord>(r => r.Title == label).List().FirstOrDefault();
        }
    }
}