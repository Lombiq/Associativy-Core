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
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GraphFactory();
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> CreateZoomedGraph(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevel, int maxZoomLevel);
        int CalculateZoomLevelCount(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int maxZoomLevel);
    }
}
