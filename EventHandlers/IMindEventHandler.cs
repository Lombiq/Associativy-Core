using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Orchard.Events;
using QuickGraph;

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
        public IUndirectedGraph<int, IUndirectedEdge<int>> IdGraph { get; private set; }

        protected MindEventContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            GraphDescriptor = graphDescriptor;
            Settings = settings;
            IdGraph = idGraph;
        }
    }

    public class AllAssociationsGraphBuiltContext : MindEventContext
    {
        public AllAssociationsGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
        }
    }

    public class SearchedGraphBuiltContext : MindEventContext
    {
        public IEnumerable<int> NodeIds { get; private set; }

        public SearchedGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IEnumerable<int> nodeIds, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
            NodeIds = nodeIds;
        }
    }
}
