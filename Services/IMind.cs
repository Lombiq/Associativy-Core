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
        /// Returns the whole association graph, containing the ids of the content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<int, IUndirectedEdge<int>> GetAllAssociations(IGraphContext graphContext, IMindSettings settings);

        /// <summary>
        /// Makes associations upon the specified nodes, containing the ids of the content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<int, IUndirectedEdge<int>> MakeAssociations(IGraphContext graphContext, IEnumerable<IContent> nodes, IMindSettings settings);

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range, containing the ids of the content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="centerNode">The node paths willl be calculated from</param>
        /// <param name="settings">Mind settings</param>
        IUndirectedGraph<int, IUndirectedEdge<int>> GetPartialGraph(IGraphContext graphContext, IContent centerNode, IMindSettings settings);
    }


    public static class MindExtensions
    {
        /// <summary>
        /// Returns the whole association graph, containing content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociationsContent(this IMind mind, IGraphEditor graphEditor, IGraphContext graphContext, IMindSettings settings)
        {
            return graphEditor.MakeContentGraph(graphContext, mind.GetAllAssociations(graphContext, settings));
        }

        /// <summary>
        /// Makes associations upon the specified nodes, containing content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociationsContent(this IMind mind, IGraphEditor graphEditor, IGraphContext graphContext, IEnumerable<IContent> nodes, IMindSettings settings)
        {
            return graphEditor.MakeContentGraph(graphContext, mind.MakeAssociations(graphContext, nodes, settings));
        }

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range, containing content items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="centerNode">The node paths willl be calculated from</param>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetPartialGraphContent(this IMind mind, IGraphEditor graphEditor, IGraphContext graphContext, IContent centerNode, IMindSettings settings)
        {
            return graphEditor.MakeContentGraph(graphContext, mind.GetPartialGraph(graphContext, centerNode, settings));
        }
    }
}
