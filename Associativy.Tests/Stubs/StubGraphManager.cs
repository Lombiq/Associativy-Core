using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Moq;
using Orchard.Tests.Stubs;

namespace Associativy.Tests.Stubs
{
    public class StubGraphManager : IGraphManager
    {
        private static MemoryConnectionManager _connectionManager;

        public StubGraphManager()
        {
            if (_connectionManager == null) _connectionManager = new MemoryConnectionManager(this, new StubCacheManager(), new Mock<IGraphEventHandler>().Object);
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
                "TestGraph",
                new Orchard.Localization.LocalizedString("Test Graph"),
                new string[] { "Page" },
                _connectionManager);
        }
    }
}
