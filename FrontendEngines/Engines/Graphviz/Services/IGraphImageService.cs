using System;
using Associativy.Models;
using Orchard.ContentManagement;
using QuickGraph;
using QuickGraph.Graphviz;

namespace Associativy.FrontendEngines.Engines.Graphviz.Services
{
    public interface IGraphImageService<TAssociativyContext>// : IDependency
     where TAssociativyContext : IAssociativyContext
    {
        string ToSvg(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, Action<GraphvizAlgorithm<IContent, IUndirectedEdge<IContent>>> initialization);
    }
}
