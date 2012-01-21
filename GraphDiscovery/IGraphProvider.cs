﻿using Associativy.Services;
using Orchard;
using Orchard.Localization;
using Associativy.GraphDiscovery;
using System.Collections.Generic;

namespace Associativy.GraphDiscovery
{
    public interface IGraphProvider : IAssociativyProvider
    {
        /// <summary>
        /// Name of the graph provider used to identify it
        /// </summary>
        string GraphName { get; }

        /// <summary>
        /// Human-readable name of the graph
        /// </summary>
        LocalizedString DisplayGraphName { get; }

        /// <summary>
        /// The types of the content items stored by the graph
        /// </summary>
        IEnumerable<string> ContentTypes { get; }

        /// <summary>
        /// The IConnectionManager instance used by the provider
        /// </summary>
        IConnectionManager ConnectionManager { get; }
    }
}
