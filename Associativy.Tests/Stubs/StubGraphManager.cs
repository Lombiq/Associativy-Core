using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Moq;
using Orchard.Tests.Stubs;
using Associativy.Tests.Helpers;
using System;
using Autofac;

namespace Associativy.Tests.Stubs
{
    public class StubGraphManager : IGraphManager
    {
        private readonly IGraphServicesFactory _graphServicesFactory;


        public StubGraphManager(IGraphServicesFactory<IMind, IConnectionManager, IPathFinder, INodeManager> graphServicesFactory)
        {
            _graphServicesFactory = graphServicesFactory;
        }


        public IGraphDescriptor FindGraph(IGraphContext graphContext)
        {
            return TestGraphDescriptor();
        }

        public IEnumerable<IGraphDescriptor> FindGraphs(IGraphContext graphContext)
        {
            return new GraphDescriptor[] { TestGraphDescriptor() };
        }

        public IEnumerable<IGraphDescriptor> FindDistinctGraphs(IGraphContext graphContext)
        {
            return new GraphDescriptor[] { TestGraphDescriptor() };
        }


        public static void Setup(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GraphServicesFactory<,,,>)).As(typeof(IGraphServicesFactory<,,,>));
        }


        private GraphDescriptor TestGraphDescriptor()
        {
            return  new GraphDescriptor(
                    TestGraphHelper.TestGraphContext().Name,
                    new Orchard.Localization.LocalizedString("Test Graph"),
                    TestGraphHelper.TestGraphContext().ContentTypes,
                    _graphServicesFactory.Factory);
        }
    }
}
