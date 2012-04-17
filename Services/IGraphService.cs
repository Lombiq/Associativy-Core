using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;
using Associativy.Models;

namespace Associativy.Services
{
    /// <summary>
    /// Performs work on graphs
    /// </summary>
    public interface IGraphService : IDependency
    {
        /// <summary>
        /// Creates a new graph object
        /// </summary>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GraphFactory();

        /// <summary>
        /// Creates a graph zoomed to the specified level from the given graph
        /// </summary>
        /// <param name="graph">The original graph</param>
        /// <param name="zoomLevel">Level to zoom into</param>
        /// <param name="zoomLevelCount">Number of zoom levels</param>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> CreateZoomedGraph(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevel, int zoomLevelCount);

        /// <summary>
        /// Computes the amount of real zoom levels for a graph for a given maximal zoom level
        /// </summary>
        /// <param name="graph">The graph to check</param>
        /// <param name="zoomLevelCount">Number of zoom levels</param>
        int CalculateZoomLevelCount(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevelCount);
    }
}
