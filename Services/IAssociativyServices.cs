using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    /// <typeparam name="TAssociativyGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IAssociativyServices<TAssociativyGraphDescriptor> : IAssociativyService
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
        /// <summary>
        /// Service for dealing with connections between nodes
        /// </summary>
        IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Performs work on graphs
        /// </summary>
        IGraphService<TAssociativyGraphDescriptor> GraphService { get; }

        /// <summary>
        /// Service for generating associations
        /// </summary>
        IMind<TAssociativyGraphDescriptor> Mind { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        INodeManager<TAssociativyGraphDescriptor> NodeManager { get; }
    }

    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    public interface IAssociativyServices : IAssociativyServices<IAssociativyGraphDescriptor>, IDependency
    {
        /// <summary>
        /// Performs work on graphs
        /// </summary>
        new IGraphService GraphService { get; }

        /// <summary>
        /// Service for generating associations
        /// </summary>
        new IMind Mind { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        new INodeManager NodeManager { get; }
    }
}
