using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Associativy.Queryable;
using Orchard.Events;

namespace Associativy.EventHandlers
{
    public interface IMindEventHandler : IEventHandler
    {
        void AllAssociationsGraphBuilt(AllAssociationsGraphBuiltContext context);
        void SearchedGraphBuilt(SearchedGraphBuiltContext context);
    }


    public abstract class MindEventContext
    {
        public IGraphDescriptor GraphDescriptor { get; private set; }
        public IMindSettings Settings { get; set; }
        public IQueryableGraph<int> IdGraph { get; private set; }

        protected MindEventContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IQueryableGraph<int> idGraph)
        {
            GraphDescriptor = graphDescriptor;
            Settings = settings;
            IdGraph = idGraph;
        }
    }

    public class AllAssociationsGraphBuiltContext : MindEventContext
    {
        public AllAssociationsGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IQueryableGraph<int> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
        }
    }

    public class SearchedGraphBuiltContext : MindEventContext
    {
        public IEnumerable<int> NodeIds { get; private set; }

        public SearchedGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IEnumerable<int> nodeIds, IQueryableGraph<int> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
            NodeIds = nodeIds;
        }
    }
}
