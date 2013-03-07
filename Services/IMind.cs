using System;
using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind
    {
        /// <summary>
        /// Returns all the associations
        /// </summary>
        /// <param name="settings">Mind settings</param>
        IMindResult GetAllAssociations(IMindSettings settings);

        /// <summary>
        /// Makes associations upon the specified nodes
        /// </summary>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IMindResult MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings);

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range
        /// </summary>
        /// <param name="centerNode">The node paths willl be calculated from</param>
        /// <param name="settings">Mind settings</param>
        IMindResult GetPartialGraph(IContent centerNode, IMindSettings settings);
    }

    public interface IMindResult
    {
        IUndirectedGraph<int, IUndirectedEdge<int>> IdGraph { get; }
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> ContentGraph { get; }
        int ActualZoomLevelCount { get; }
    }

    public class MindResult : IMindResult
    {
        private readonly Lazy<IUndirectedGraph<int, IUndirectedEdge<int>>> _unzoomedIdGraphLazy;
        public IUndirectedGraph<int, IUndirectedEdge<int>> UnzoomedIdGraph { get { return _unzoomedIdGraphLazy.Value; } }

        private readonly Lazy<IUndirectedGraph<int, IUndirectedEdge<int>>> _idGraphLazy;
        public IUndirectedGraph<int, IUndirectedEdge<int>> IdGraph { get { return _idGraphLazy.Value; } }

        private readonly Lazy<IUndirectedGraph<IContent, IUndirectedEdge<IContent>>> _contentGraphLazy;
        public IUndirectedGraph<IContent, IUndirectedEdge<IContent>> ContentGraph { get { return _contentGraphLazy.Value; } }

        private readonly Lazy<int> _actualZoomLevelCountLazy;
        public int ActualZoomLevelCount { get { return _actualZoomLevelCountLazy.Value; } }


        public MindResult(Func<MindResult, IUndirectedGraph<int, IUndirectedEdge<int>>> unzoomedIdGraphFactory, Func<MindResult, IUndirectedGraph<int, IUndirectedEdge<int>>> idGraphFactory, Func<MindResult, IUndirectedGraph<IContent, IUndirectedEdge<IContent>>> contentGraphFactory, Func<MindResult, int> actualZoomLevelCountFactory)
        {
            _unzoomedIdGraphLazy = new Lazy<IUndirectedGraph<int, IUndirectedEdge<int>>>(() => unzoomedIdGraphFactory(this));
            _idGraphLazy = new Lazy<IUndirectedGraph<int, IUndirectedEdge<int>>>(() => idGraphFactory(this));
            _contentGraphLazy = new Lazy<IUndirectedGraph<IContent, IUndirectedEdge<IContent>>>(() => contentGraphFactory(this));
            _actualZoomLevelCountLazy = new Lazy<int>(() => actualZoomLevelCountFactory(this));
        }
    }
}
