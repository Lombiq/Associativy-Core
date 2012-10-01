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
            return new GraphContext { GraphName = "TestGraph", ContentTypes = new string[] { "Notion" } };
        }

        public static NotionGraphBuilder BuildTestGraph(IContainer container, IGraphContext graphContext = null)
        {
            graphContext = graphContext ?? TestGraphContext();
            var connectionManager = container.Resolve<IGraphManager>().FindGraph(graphContext).PathServices.ConnectionManager;

            var graphBuilder = new NotionGraphBuilder(container.Resolve<IContentManager>(), graphContext, connectionManager);
            graphBuilder.Build(false, false);
            return graphBuilder;
        }
    }
}
