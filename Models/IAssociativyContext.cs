using Associativy.Services;
using Orchard;
using Orchard.Localization;

namespace Associativy.Models
{
    // Maybe also set the search form's content type from here?
    public interface IAssociativyContext : IDependency
    {
        /// <summary>
        /// Human-readable name of the graph.
        /// </summary>
        LocalizedString GraphName { get; }

        /// <summary>
        /// Name of the graph used to identify it. Must be unique across the registered contexts.
        /// </summary>
        string TechnicalGraphName { get; }

        /// <summary>
        /// The types of the content items stored by the graph.
        /// </summary>
        string[] ContentTypes { get; }

        /// <summary>
        /// Upper bound of the graph zoom levels (lower bound is always zero).
        /// </summary>
        int MaxZoomLevel { get; }

        /// <summary>
        /// The IConnectionManager instance used by the context.
        /// </summary>
        IConnectionManager ConnectionManager { get; }
    }
}
