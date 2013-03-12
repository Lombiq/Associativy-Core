
namespace Associativy.Services
{
    /// <summary>
    /// Contains services that are specific to a graph.
    /// </summary>
    public interface IGraphServices
    {
        /// <summary>
        /// Service for generating associations
        /// </summary>
        IMind Mind { get; }

        /// <summary>
        /// Service for dealing with connections between nodes
        /// </summary>
        IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Deals with node-to-node path calculations
        /// </summary>
        IPathFinder PathFinder { get; }

        /// <summary>
        /// Service for handling nodes
        /// </summary>
        INodeManager NodeManager { get; }
    }
}