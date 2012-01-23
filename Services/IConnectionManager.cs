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
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        /// <returns>True if the nodes are neighbours; False if they aren't</returns>
        bool AreNeighbours(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="node1">The first node</param>
        /// <param name="node2">The second node</param>
        void Connect(IGraphContext graphContext, IContent node1, IContent node2);

        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        void Connect(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="node">The node</param>
        void DeleteFromNode(IGraphContext graphContext, IContent node);

        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="nodeId">The node's id</param>
        void DeleteFromNode(IGraphContext graphContext, int nodeId);

        void Disconnect(IGraphContext graphContext, IContent node1, IContent node2);

        void Disconnect(IGraphContext graphContext, int nodeId1, int nodeId2);


        /// <summary>
        /// Returns all connector objects
        /// </summary>
        /// <returns>All connector objects</returns>
        IEnumerable<INodeToNodeConnector> GetAll(IGraphContext graphContext);

        /// <summary>
        /// Returns the ids of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The ids of all the directly connected (= neighbour) nodes</returns>
        IEnumerable<int> GetNeighbourIds(IGraphContext graphContext, int nodeId);

        /// <summary>
        /// Returns the count of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The count of all the directly connected (= neighbour) nodes</returns>
        int GetNeighbourCount(IGraphContext graphContext, int nodeId);
    }

    /// <summary>
    /// Service for dealing with connections between nodes
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public interface IConnectionManager<TNodeToNodeConnectorRecord> : IConnectionManager
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
    }
}
