using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Associativy.Queryable;
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
        /// Returns the whole association graph, containing the ids of the content items
        /// </summary>
        /// <param name="settings">Mind settings</param>
        IQueryableGraph<int> GetAllAssociations(IMindSettings settings);

        /// <summary>
        /// Makes associations upon the specified nodes, containing the ids of the content items
        /// </summary>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IQueryableGraph<int> MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings);
    }


    public static class MindExtensions
    {
        /// <summary>
        /// Returns the whole association graph, containing content items
        /// </summary>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociationsContent(this IMind mind, INodeManager nodeManager, IMindSettings settings)
        {
            return nodeManager.MakeContentGraph(mind.GetAllAssociations(settings).ToGraph());
        }

        /// <summary>
        /// Makes associations upon the specified nodes, containing content items
        /// </summary>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociationsContent(this IMind mind, INodeManager nodeManager, IEnumerable<IContent> nodes, IMindSettings settings)
        {
            return nodeManager.MakeContentGraph(mind.MakeAssociations(nodes, settings).ToGraph());
        }

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range, containing content items
        /// </summary>
        /// <param name="centerNode">The node paths willl be calculated from</param>
        /// <param name="settings">Mind settings</param>
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetPartialGraphContent(this IMind mind, INodeManager nodeManager, IContent centerNode, IMindSettings settings)
        {
            return nodeManager.MakeContentGraph(mind.GetPartialGraph(centerNode, settings).ToGraph());
        }
    }
}
