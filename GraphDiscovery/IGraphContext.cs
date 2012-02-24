using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Describes under what circumstances is a graph used
    /// </summary>
    /// <remarks>
    /// For frontend engines to work properly, IGraphContext implementations should be marked with the Serializable attribute.
    /// </remarks>
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

    [OrchardFeature("Associativy")]
    public static class GraphContextExtensions
    {
        public static string Stringify(this IGraphContext graphContext)
        {
            return graphContext.GraphName + String.Join(" ", graphContext.ContentTypes);
        }
    }
}