using Associativy.Services;
using Orchard;
using Orchard.Localization;

namespace Associativy.Models
{
    // Maybe also set the search form's content type from here?
    public interface IAssociativyGraphDescriptor : IDependency
    {
        /// <summary>
        /// Human-readable name of the graph.
        /// </summary>
        LocalizedString GraphName { get; }

        /// <summary>
        /// Name of the graph used to identify it. Must be unique across the registered graphDescriptors.
        /// </summary>
        string TechnicalGraphName { get; }

        /// <summary>
        /// The types of the content items stored by the graph.
        /// </summary>
        string[] ContentTypes { get; }

        /// <summary>
        /// The IConnectionManager instance used by the graphDescriptor.
        /// </summary>
        IConnectionManager ConnectionManager { get; }
    }
}
