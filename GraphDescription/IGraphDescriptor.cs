using Associativy.Services;
using Orchard;
using Orchard.Localization;
using Associativy.GraphDescription;
using System.Collections.Generic;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the graph in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    public interface IGraphDescriptor : IDependency
    {
        /// <summary>
        /// Name of the graph used to identify it. Must be unique across the registered graphDescriptors.
        /// </summary>
        string GraphName { get; }

        /// <summary>
        /// Human-readable name of the graph.
        /// </summary>
        LocalizedString DisplayName { get; }

        /// <summary>
        /// The types of the content items stored by the graph.
        /// </summary>
        string[] ContentTypes { get; }

        /// <summary>
        /// The IConnectionManager instance used by the graphDescriptor.
        /// </summary>
        IConnectionManager ConnectionManager { get; }

        IGraphContext SupportedGraphContext { get; }

        IEnumerable<IContentContext> SupportedContentContexts { get; }
    }
}
