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
        public IGraphDescriptor GraphDescriptor { get; private set; }
        public IContentQuery<ContentItem> Query { get; private set; }


        public QueryBuiltContext(IGraphDescriptor graphDescriptor, IContentQuery<ContentItem> query)
        {
            GraphDescriptor = graphDescriptor;
            Query = query;
        }
    }
}
