using Associativy.GraphDiscovery;
using Associativy.Instances.Notions;
using Autofac;
using Orchard.ContentManagement;

namespace Associativy.Tests
{
    class TestGraphHelper
    {
        public static IGraphContext TestGraphContext()
        {
            return new GraphContext { GraphName = "TestGraph", ContentTypes = new string[] { "Notion" } };
        }

        public static NotionGraphBuilder BuildTestGraph(IContainer container)
        {
            var graphContext = TestGraphContext();
            var connectionManager = container.Resolve<IGraphManager>().FindGraph(graphContext).PathServices.ConnectionManager;

            var graphBuilder = new NotionGraphBuilder(container.Resolve<IContentManager>(), graphContext, connectionManager);
            graphBuilder.Build(false);
            return graphBuilder;
        }
    }
}
