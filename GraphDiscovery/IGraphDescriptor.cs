using System.Collections.Generic;
using Associativy.Services;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Describes the capabilities of an associative graph
    /// </summary>
    public interface IGraphDescriptor : IGraph
    {
        IGraphServices Services { get; }
    }


    public static class GraphDescriptorExtensions
    {
        public static IGraphContext MaximalContext(this IGraphDescriptor descriptor)
        {
            return new GraphContext { Name = descriptor.Name, ContentTypes = descriptor.ContentTypes };
        }
    }
}
