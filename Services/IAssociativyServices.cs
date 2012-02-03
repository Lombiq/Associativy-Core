using Associativy.Models;
using Orchard;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    public interface IAssociativyServices : IDependency
    {
        /// <summary>
        /// Handles registered graphs
        /// </summary>
        IGraphManager GraphManager { get; }

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
}
