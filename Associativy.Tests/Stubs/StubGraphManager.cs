using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Moq;
using Orchard.Tests.Stubs;
using Associativy.Tests.Helpers;

namespace Associativy.Tests.Stubs
{
    public class StubGraphManager : IGraphManager
    {
        private static PathServices _pathServices;

        public StubGraphManager()
        {
            if (_pathServices == null) _pathServices = new PathServices(
                new MemoryConnectionManager(this, new StubCacheManager(), new Mock<IGraphEventHandler>().Object),
                new StandardPathFinder(this, new StubGraphEditor(), new Mock<IGraphEventMonitor>().Object, new StubCacheManager()));
        }

        public GraphDescriptor FindGraph(IGraphContext graphContext)
        {
            return TestGraphDescriptor();
        }

        public IEnumerable<GraphDescriptor> FindGraphs(IGraphContext graphContext)
        {
            return new GraphDescriptor[] { TestGraphDescriptor() };
        }

        public IEnumerable<GraphDescriptor> FindDistinctGraphs(IGraphContext graphContext)
        {
            return new GraphDescriptor[] { TestGraphDescriptor() };
        }

        private GraphDescriptor TestGraphDescriptor()
        {
            return new GraphDescriptor(
                TestGraphHelper.TestGraphContext().GraphName,
                new Orchard.Localization.LocalizedString("Test Graph"),
                TestGraphHelper.TestGraphContext().ContentTypes,
                () => _pathServices);
        }
    }
}
