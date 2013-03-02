using Associativy.GraphDiscovery;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Performs work on graphs
    /// </summary>
    public interface IGraphEditor : IDependency
    {
        /// <summary>
        /// Creates a new graph object
        /// </summary>
        IMutableUndirectedGraph<TNode, IUndirectedEdge<TNode>> GraphFactory<TNode>();

        /// <summary>
        /// Creates a graph zoomed to the specified level from the given graph
        /// </summary>
        /// <param name="graph">The original graph</param>
        /// <param name="zoomLevel">Level to zoom into</param>
        /// <param name="zoomLevelCount">Number of zoom levels</param>
        IUndirectedGraph<TNode, IUndirectedEdge<TNode>> CreateZoomedGraph<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int zoomLevel, int zoomLevelCount);

        /// <summary>
        /// Computes the amount of actual zoom levels for a graph for a given maximal zoom level
        /// </summary>
        /// <param name="graph">The graph to check</param>
        /// <param name="maximalZoomLevelCount">Number of zoom levels</param>
        int CalculateZoomLevelCount<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int maximalZoomLevelCount);

        /// <summary>
        /// Creates a graph of content items from a graph of content item IDs
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="idGraph">Graph of the IDs</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
    }
}
