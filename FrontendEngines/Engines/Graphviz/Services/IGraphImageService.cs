using System;
using Associativy.Models;
using Orchard.ContentManagement;
using QuickGraph;
using QuickGraph.Graphviz;
using Orchard;
using Associativy.Services;

namespace Associativy.FrontendEngines.Engines.Graphviz.Services
{
    public interface IGraphImageService : IAssociativyService, IDependency
    {
        string ToSvg(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, Action<GraphvizAlgorithm<IContent, IUndirectedEdge<IContent>>> initialization);
    }
}
