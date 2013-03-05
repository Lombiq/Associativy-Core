using Associativy.GraphDiscovery;
using Associativy.Instances.Notions;
using Autofac;
using Orchard.ContentManagement;

namespace Associativy.Tests.Helpers
{
    public class TestGraphHelper
    {
        public static IGraphContext TestGraphContext()
        {
            return new GraphContext { Name = "TestGraph", ContentTypes = new string[] { "Notion" } };
        }

        public static NotionGraphBuilder BuildTestGraph(IContentManager contentManager, IGraphDescriptor graphDescriptor)
        {
            var graphBuilder = new NotionGraphBuilder(contentManager, graphDescriptor);
            graphBuilder.Build(false, false);
            return graphBuilder;
        }
    }
}
