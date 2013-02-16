using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Orchard.Events;

namespace Associativy.EventHandlers
{
    public interface INodeManagerEventHander : IEventHandler
    {
        void QueryBuilt(QueryBuiltContext context);
    }


    public class QueryBuiltContext
    {
        public IGraphContext GraphContext { get; private set; }
        public IContentQuery<ContentItem> Query { get; private set; }


        public QueryBuiltContext(IGraphContext graphContext, IContentQuery<ContentItem> query)
        {
            GraphContext = graphContext;
            Query = query;
        }
    }
}
