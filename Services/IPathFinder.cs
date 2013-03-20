using System.Collections.Generic;
using Associativy.Models.Services;
using Associativy.Queryable;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Calculates all paths between two nodes, depending on the settings.
        /// </summary>
        /// <param name="node1Id"></param>
        /// <param name="node2Id"></param>
        /// <param name="settings"></param>
        IPathResult FindPaths(int node1Id, int node2Id, IPathFinderSettings settings);

        /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range, containing the ids of the content items
        /// </summary>
        /// <param name="centralNodeId">The node paths will be calculated from</param>
        /// <param name="settings"></param>
        IQueryableGraph<int> GetPartialGraph(int centralNodeId, IPathFinderSettings settings);
    }

    public interface IPathResult
    {
        IQueryableGraph<int> SucceededGraph { get; }
        IEnumerable<IEnumerable<int>> SucceededPaths { get; }
    }

    public static class PathFinderExtensions
	{
                /// <summary>
        /// Returns a partial graph of the graph that starts from the center node and contains all paths within the specified range, containing the ids of the content items
        /// </summary>
        /// <param name="centralNodeId">The node paths will be calculated from</param>
        /// <param name="settings"></param>
        public static IQueryableGraph<int> GetPartialGraph(this IPathFinder pathFinder, IContent centralNode, IPathFinderSettings settings)
        {
            return pathFinder.GetPartialGraph(centralNode.ContentItem.Id, settings);
        }
	}
}
