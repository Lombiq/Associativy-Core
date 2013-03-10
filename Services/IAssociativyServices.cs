using Associativy.GraphDiscovery;
using Associativy.Queryable;
using Orchard;

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
        IGraphEditor GraphEditor { get; }

        /// <summary>
        /// Creates IQueryableGraph instances
        /// </summary>
        IQueryableGraphFactory QueryableGraphFactory { get; }
    }
}
