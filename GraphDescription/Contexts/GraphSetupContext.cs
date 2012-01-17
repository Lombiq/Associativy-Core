using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Services;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public abstract class GraphSetupContext
    {
        /// <summary>
        /// The types of the content items stored by the graph.
        /// </summary>
        public abstract IList<string> ContentTypes { get; }

        /// <summary>
        /// The IConnectionManager instance used to build the graph
        /// </summary>
        public virtual IConnectionManager ConnectionManager { get; set; }
    }
}