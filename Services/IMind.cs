using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.Mind;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind : IDependency
    {
        /// <summary>
        /// Returns the whole association graph
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IGraphContext graphContext, IMindSettings settings = null);

        /// <summary>
        /// Makes associations upon the specified nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IGraphContext graphContext, IEnumerable<IContent> nodes, IMindSettings settings = null);

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="centerNode">The node paths willl be calculated from</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetPartialGraph(IGraphContext graphContext, IContent centerNode, IMindSettings settings = null);
    }
}
