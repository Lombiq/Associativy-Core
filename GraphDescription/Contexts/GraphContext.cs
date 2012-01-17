using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public abstract class GraphContext
    {
        /// <summary>
        /// Name of the graph used to identify it
        /// </summary>
        public abstract string GraphName { get; }
    }
}