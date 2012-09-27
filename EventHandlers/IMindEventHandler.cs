using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Events;
using QuickGraph;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;

namespace Associativy.EventHandlers
{
    public interface IMindEventHandler : IEventHandler
    {
        void BeforeWholeContentGraphBuilding(IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
        void BeforePartialContentGraphBuilding(IGraphContext graphContext, IContent centerNode, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
        void BeforeSearchedContentGraphBuilding(IGraphContext graphContext, IEnumerable<IContent> nodes, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
    }
}
