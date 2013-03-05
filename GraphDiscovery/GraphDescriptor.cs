using System;
using System.Collections.Generic;
using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Describes the capabilities of an associative graph
    /// </summary>
    [OrchardFeature("Associativy")]
    public class GraphDescriptor : IGraphDescriptor
    {
        public string Name { get; private set; }
        public LocalizedString DisplayName { get; private set; }
        public IEnumerable<string> ContentTypes { get; private set; }

        private readonly Lazy<IGraphServices> _graphServicesField;
        public IGraphServices Services { get { return _graphServicesField.Value; } }


        public GraphDescriptor(string name, LocalizedString displayName, IEnumerable<string> contentTypes, Func<IGraphDescriptor, IGraphServices> servicesFactory)
        {
            Name = name;
            DisplayName = displayName;
            ContentTypes = contentTypes;
            _graphServicesField = new Lazy<IGraphServices>(() => servicesFactory(this));
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