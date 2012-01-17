using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    /// <typeparam name="TGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IAssociativyServices<TGraphDescriptor> : IAssociativyService
        where TGraphDescriptor : IGraphDescriptor
    {
        /// <summary>
        /// Service for dealing with connections between nodes
        /// </summary>
        IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Performs work on graphs
        /// </summary>
        IGraphService<TGraphDescriptor> GraphService { get; }

        /// <summary>
        /// Service for generating associations
        /// </summary>
        IMind<TGraphDescriptor> Mind { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        INodeManager<TGraphDescriptor> NodeManager { get; }
    }

    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    public interface IAssociativyServices : IAssociativyServices<IGraphDescriptor>, IDependency
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
