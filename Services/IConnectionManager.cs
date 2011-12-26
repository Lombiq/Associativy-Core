﻿using System;
using System.Collections.Generic;
using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public interface IConnectionManager<TNodeToNodeConnectorRecord> : IDependency
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        /// <summary>
        /// Checks if the nodes are neighbours (= directly connected to each other)
        /// </summary>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        /// <returns>True if the nodes are neighbours; False if they aren't</returns>
        bool AreNeighbours(int nodeId1, int nodeId2);


        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="node1">The first node</param>
        /// <param name="node2">The second node</param>
        void Connect(INode node1, INode node2);

        /// <summary>
        /// Creates a new connection between two nodes
        /// </summary>
        /// <param name="nodeId1">Id of the first node</param>
        /// <param name="nodeId2">Id of the second node</param>
        void Connect(int nodeId1, int nodeId2);


        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="node">The node</param>
        void DeleteFromNode(INode node);

        /// <summary>
        /// Deletes all connections of the node
        /// </summary>
        /// <param name="nodeId">The node's id</param>
        void DeleteFromNode(int nodeId);

        /// <summary>
        /// Deletes the connection
        /// </summary>
        /// <param name="id">Id of the connection</param>
        void Delete(int id);


        /// <summary>
        /// Returns all connector records
        /// </summary>
        /// <returns>All connector records</returns>
        IList<TNodeToNodeConnectorRecord> GetAll();

        /// <summary>
        /// Returns the ids of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The ids of all the directly connected (= neighbour) nodes</returns>
        IList<int> GetNeighbourIds(int nodeId);

        /// <summary>
        /// Returns the count of all the directly connected (= neighbour) nodes
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <returns>The count of all the directly connected (= neighbour) nodes</returns>
        int GetNeighbourCount(int nodeId);
    }
}
