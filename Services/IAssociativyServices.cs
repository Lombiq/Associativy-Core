using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    public interface IAssociativyServices : IAssociativyService, IDependency
    {
        /// <summary>
        /// Service for dealing with connections between nodes
        /// </summary>
        IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Service for generating associations
        /// </summary>
        IMind Mind { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        INodeManager NodeManager { get; }
    }

    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    /// <typeparam name="TAssociativyGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyGraphDescriptor> : IAssociativyServices
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
    }
}
