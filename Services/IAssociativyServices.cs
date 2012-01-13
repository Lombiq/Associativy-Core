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
        /// Performs work on graphs
        /// </summary>
        IGraphService GraphService { get; }

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
    /// <typeparam name="TAssociativyGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IAssociativyServices<TAssociativyGraphDescriptor> : IAssociativyServices
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
    }
}
