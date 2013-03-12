using System.Collections.Generic;
using System.Linq;
using Associativy.Models.Services;
using Associativy.Queryable;
using Orchard.ContentManagement;

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
        /// <param name="nodeIds">The ids of the nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IQueryableGraph<int> MakeAssociations(IEnumerable<int> nodeIds, IMindSettings settings);
    }


    public static class MindExtensions
    {
        /// <summary>
        /// Makes associations upon the specified nodes, containing the ids of the content items
        /// </summary>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        public static IQueryableGraph<int> MakeAssociations(this IMind mind, IEnumerable<IContent> nodes, IMindSettings settings)
        {
            return mind.MakeAssociations(nodes.Select(node => node.ContentItem.Id), settings);
        }
    }
}
