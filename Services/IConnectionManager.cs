using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System;
using Associativy.Events;

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
        /// Event handler that is triggered when something changes in the graph
        /// </summary>
        event EventHandler<GraphEventArgs> GraphChanged;


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
        /// <param name="nodeId1"></param>
        /// <param name="nodeId2"></param>
        void Add(int nodeId1, int nodeId2);
        void Add(INode node1, INode node2);
        void DeleteMany(int nodeId);
        void Delete(int id);
        IList<TNodeToNodeConnectorRecord> GetAll();
        IList<int> GetNeighbourIds(int nodeId);
        int GetNeighbourCount(int nodeId);
    }
}
