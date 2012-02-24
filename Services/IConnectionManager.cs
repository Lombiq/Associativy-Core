using System.Collections.Generic;
using Associativy.Models;
using Orchard.ContentManagement;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Checks if the nodes are neighbours (= directly connected to each other)
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        /// <returns>True if the nodes are neighbours; False if they aren't</returns>
        bool AreNeighbours(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        void Connect(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId">The node's id</param>
        void DeleteFromNode(IGraphContext graphContext, int nodeId);

        /// <summary>
        /// Removes the connection between two nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        void Disconnect(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Returns all connector objects
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <returns>All connector objects</returns>
        IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext);

        /// <summary>
        /// Returns the ids of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The ids of all the directly connected (= neighbour) nodes</returns>
        IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId);

        /// <summary>
        /// Returns the count of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The count of all the directly connected (= neighbour) nodes</returns>
        int GetNeighbourCount(IGraphContext graphContext, int nodeId);
    }
}
