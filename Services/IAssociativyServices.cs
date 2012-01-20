using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    public interface IAssociativyServices : IDependency
    {
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
