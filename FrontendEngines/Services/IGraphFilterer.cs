using System;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;
using System.Collections.Generic;
using Associativy.FrontendEngines.ViewModels;

namespace Associativy.FrontendEngines.Services
{
    public interface IGraphFilterer : IDependency
    {
        IDictionary<int, INodeViewModel> RunFilters(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, string frontendEngine);
    }
}
