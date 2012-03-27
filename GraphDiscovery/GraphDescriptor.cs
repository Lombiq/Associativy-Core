using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using System.Data;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Describes the capabilities of an associative graph
    /// </summary>
    [OrchardFeature("Associativy")]
    public class GraphDescriptor
    {
        public string GraphName { get; private set; }
        public LocalizedString DisplayGraphName { get; private set; }
        public IEnumerable<string> ContentTypes { get; private set; }
        public IConnectionManager ConnectionManager { get; private set; }

        public GraphDescriptor(string name, LocalizedString displayName, IEnumerable<string> contentTypes, IConnectionManager connectionManager)
        {
            GraphName = name;
            DisplayGraphName = displayName;
            ContentTypes = contentTypes;
            ConnectionManager = connectionManager;
        }
    }

    //[OrchardFeature("Associativy")]
    //public static class GraphDescriptorExtensions
    //{
    //    /// <summary>
    //    /// Creates the maximal context the descriptor supports
    //    /// </summary>
    //    public static IGraphContext ProduceMaximalContext(this GraphDescriptor descriptor)
    //    {
    //        return new GraphContext { GraphName = descriptor.GraphName, ContentTypes = descriptor.ContentTypes };
    //    }
    //}
}