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
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        /// <returns>True if the nodes are neighbours; False if they aren't</returns>
        bool AreNeighbours(IGraphContext graphContext, int node1Id, int node2Id);


        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        void Connect(IGraphContext graphContext, int node1Id, int node2Id);


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
        /// <param name="node1Id">Id of the first node</param>
        /// <param name="node2Id">Id of the second node</param>
        void Disconnect(IGraphContext graphContext, int node1Id, int node2Id);


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

    public static class ConnectionManagerExtensions
    {
        public static void Connect<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Connect(graphContext, node1.ContentItem.Id, node2.ContentItem.Id);
        }

        public static void DeleteFromNode<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.DeleteFromNode(graphContext, node.ContentItem.Id);
        }

        public static void Disconnect<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Disconnect(graphContext, node1.ContentItem.Id, node2.ContentItem.Id);
        }

        public static IEnumerable<int> GetNeighbourIds<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourIds(graphContext, node.ContentItem.Id);
        }

        public static int GetNeighbourCount<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourCount(graphContext, node.ContentItem.Id);
        }
    }
}
