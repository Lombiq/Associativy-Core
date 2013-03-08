using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard.ContentManagement;

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
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        /// <returns>True if the nodes are neighbours; False if they aren't</returns>
        bool AreNeighbours(int node1Id, int node2Id);


        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        void Connect(int node1Id, int node2Id);


        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="nodeId">The node's id</param>
        void DeleteFromNode(int nodeId);

        /// <summary>
        /// Removes the connection between two nodes
        /// </summary>
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        void Disconnect(int node1Id, int node2Id);


        /// <summary>
        /// Returns all connector objects
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="skip">Number of items to skip from the beginning of the sequence</param>
        /// <param name="count">The maximal number of items to return</param>
        /// <returns>All connector objects</returns>
        IEnumerable<INodeToNodeConnector> GetAll(int skip, int count);

        /// <summary>
        /// Returns the ids of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <param name="skip">Number of items to skip from the beginning of the sequence</param>
        /// <param name="count">The maximal number of items to return</param>
        /// <returns>The ids of all the directly connected (= neighbour) nodes</returns>
        IEnumerable<int> GetNeighbourIds(int nodeId, int skip, int count);

        /// <summary>
        /// Returns the count of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The count of all the directly connected (= neighbour) nodes</returns>
        int GetNeighbourCount(int nodeId);
    }

    public static class ConnectionManagerExtensions
    {
        public static void Connect<TConnectionManager>(this TConnectionManager connectionManager, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Connect(node1.ContentItem.Id, node2.ContentItem.Id);
        }

        public static void DeleteFromNode<TConnectionManager>(this TConnectionManager connectionManager, IContent node)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.DeleteFromNode(node.ContentItem.Id);
        }

        public static void Disconnect<TConnectionManager>(this TConnectionManager connectionManager, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Disconnect(node1.ContentItem.Id, node2.ContentItem.Id);
        }

        public static IEnumerable<int> GetNeighbourIds<TConnectionManager>(this TConnectionManager connectionManager, IContent node, int skip, int count)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourIds(node.ContentItem.Id, skip, count);
        }

        public static int GetNeighbourCount<TConnectionManager>(this TConnectionManager connectionManager, IContent node)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourCount(node.ContentItem.Id);
        }
    }
}
