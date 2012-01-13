using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations
    /// </summary>
    public interface IPathFinder : IAssociativyService, IDependency
    {
        /// <summary>
        /// Calculates all paths between two nodes, depending on the settings.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <param name="settings"></param>
        /// <returns>A list of succeeded paths, where a path is a list of the ids of the nodes on the path.</returns>
        IEnumerable<IEnumerable<int>> FindPaths(int startNodeId, int targetNodeId, IMindSettings settings);
    }

    /// <summary>
    /// Deals with node-to-node path calculations
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    /// <typeparam name="TAssociativyGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IPathFinder<TNodeToNodeConnectorRecord, TAssociativyGraphDescriptor> : IPathFinder
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
    }
}
