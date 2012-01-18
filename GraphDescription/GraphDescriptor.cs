using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDescription
{
    [OrchardFeature("Associativy")]
    public class GraphDescriptor
    {
        /// <summary>
        /// Human-readable name of the graph
        /// </summary>
        LocalizedString DisplayName { get; set; }

        /// <summary>
        /// Name of the graph used to identify it. Must be unique across the registered graphDescriptors.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The types of the content items stored by the graph
        /// </summary>
        string[] ContentTypes { get; set; }

        /// <summary>
        /// The IConnectionManager instance used by the graph
        /// </summary>
        IConnectionManager ConnectionManager { get; set; }
    }
}