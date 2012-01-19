using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.GraphDiscovery
{
    public interface IGraphContext
    {
        /// <summary>
        /// Name of the graph used to identify it
        /// </summary>
        string GraphName { get; }

        /// <summary>
        /// The types of the content items stored by the graph
        /// </summary>
        IEnumerable<string> ContentTypes { get; }
    }
}