using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Events;
using QuickGraph;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Associativy.Models.Services;

namespace Associativy.EventHandlers
{
    public interface IMindEventHandler : IEventHandler
    {
        void AllAssociationsGraphBuilt(AllAssociationsGraphBuiltContext context);
        void PartialGraphBuilt(PartialGraphBuiltContext context);
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

    public class PartialGraphBuiltContext : MindEventContext
    {
        public IContent CenterNode { get; private set; }

        public PartialGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IContent centerNode, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
            CenterNode = centerNode;
        }
    }

    public class SearchedGraphBuiltContext : MindEventContext
    {
        public IEnumerable<IContent> Nodes { get; private set; }

        public SearchedGraphBuiltContext(IGraphDescriptor graphDescriptor, IMindSettings settings, IEnumerable<IContent> nodes, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphDescriptor, settings, idGraph)
        {
            Nodes = nodes;
        }
    }
}
