using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Events;
using QuickGraph;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Associativy.Models.Mind;

namespace Associativy.EventHandlers
{
    public interface IMindEventHandler : IEventHandler
    {
        void BeforeWholeContentGraphBuilding(BeforeWholeContentGraphBuildingContext context);
        void BeforePartialContentGraphBuilding(BeforePartialContentGraphBuildingContext context);
        void BeforeSearchedContentGraphBuilding(BeforeSearchedContentGraphBuildingContext context);
    }


    public abstract class MindEventContext
    {
        public IGraphContext GraphContext { get; private set; }
        public IMindSettings Settings { get; set; }
        public IUndirectedGraph<int, IUndirectedEdge<int>> IdGraph { get; private set; }

        protected MindEventContext(IGraphContext graphContext, IMindSettings settings, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            GraphContext = graphContext;
            Settings = settings;
            IdGraph = idGraph;
        }
    }

    public class BeforeWholeContentGraphBuildingContext : MindEventContext
    {
        public BeforeWholeContentGraphBuildingContext(IGraphContext graphContext, IMindSettings settings, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphContext, settings, idGraph)
        {
        }
    }

    public class BeforePartialContentGraphBuildingContext : MindEventContext
    {
        public IContent CenterNode { get; private set; }

        public BeforePartialContentGraphBuildingContext(IGraphContext graphContext, IMindSettings settings, IContent centerNode, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphContext, settings, idGraph)
        {
            CenterNode = centerNode;
        }
    }

    public class BeforeSearchedContentGraphBuildingContext : MindEventContext
    {
        public IEnumerable<IContent> Nodes { get; private set; }

        public BeforeSearchedContentGraphBuildingContext(IGraphContext graphContext, IMindSettings settings, IEnumerable<IContent> nodes, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
            : base(graphContext, settings, idGraph)
        {
            Nodes = nodes;
        }
    }
}
