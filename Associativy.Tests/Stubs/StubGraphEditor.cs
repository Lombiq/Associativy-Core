using Associativy.Services;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Tests.Stubs
{
    public class StubGraphEditor : IGraphEditor
    {
        public IMutableUndirectedGraph<TNode, IUndirectedEdge<TNode>> GraphFactory<TNode>()
        {
            return new UndirectedGraph<TNode, IUndirectedEdge<TNode>>(false);
        }

        public virtual IUndirectedGraph<TNode, IUndirectedEdge<TNode>> CreateZoomedGraph<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int zoomLevel, int zoomLevelCount)
        {
            return graph;
        }

        public virtual int CalculateZoomLevelCount<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int zoomLevelCount)
        {
            return 1;
        }


        public IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(GraphDiscovery.IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            return GraphFactory<IContent>();
        }
    }
}
