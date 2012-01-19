using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    /// <summary>
    /// Describes the graph in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    public abstract class GraphDescriptor
    {
        /// <summary>
        /// Name of the graph provider used to identify it
        /// </summary>
        public virtual string GraphName { get; set; }

        /// <summary>
        /// Human-readable name of the graph
        /// </summary>
        public virtual LocalizedString DisplayGraphName { get; set; }

        /// <summary>
        /// The types of the content items stored by the graph
        /// </summary>
        public virtual IEnumerable<string> ContentTypes { get; set; }

        /// <summary>
        /// The IConnectionManager instance used by the provider
        /// </summary>
        public virtual IConnectionManager ConnectionManager { get; set; }
    }
}