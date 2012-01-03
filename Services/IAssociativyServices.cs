using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    /// <typeparam name="TAssociativyContext">Type of the IAssociativyContext to use</typeparam>
    public interface IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext>// : IDependency
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        IAssociativyContext Context { get; }

        /// <summary>
        /// Service for dealing with connections between nodes
        /// </summary>
        IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> ConnectionManager { get; }

        /// <summary>
        /// Service for generating associations
        /// </summary>
        IMind<TNodeToNodeConnectorRecord, TAssociativyContext> Mind { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        INodeManager<TAssociativyContext> NodeManager { get; }
    }
}
