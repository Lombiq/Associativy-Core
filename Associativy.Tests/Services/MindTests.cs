using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Associativy.Tests.Stubs;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata;
using Orchard.Data;
using Orchard.FileSystems.AppData;
using Orchard.Tests;
using Orchard.Tests.ContentManagement;
using Orchard.Tests.Stubs;
using Orchard.Tests.UI.Navigation;
using Orchard.Tests.Utility;
using QuickGraph;
using Associativy.Tests.Helpers;
using Associativy.Models.Services;
using System;
using Orchard.Caching.Services;
using Associativy.Queryable;

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class MindTests : ContentManagerEnabledTestBase
    {
        private IGraphDescriptor _graphDescriptor;
        private IContentManager _contentManager;

        [SetUp]
        public override void Init()
        {
            base.Init();

            var builder = new ContainerBuilder();

            var cacheService = new StubCacheService();
            builder.RegisterInstance(new GraphEventMonitor(cacheService)).As<IGraphEventMonitor>();
            builder.RegisterInstance(new StubNodeManager()).As<INodeManager>();
            builder.RegisterInstance(new StubGraphEditor()).As<IGraphEditor>();
            builder.RegisterInstance(cacheService).As<ICacheService>();
            builder.RegisterInstance(new StubQueryableGraphFactory()).As<IQueryableGraphFactory>();
            builder.RegisterType<GraphCacheService>().As<IGraphCacheService>();
            builder.RegisterInstance(new StubCacheService()).As<ICacheService>();
            builder.RegisterType<StubGraphManager>().As<IGraphManager>();
            builder.RegisterType<StandardMind>().As<IMind>();
            builder.RegisterType<MemoryConnectionManager>().As<IConnectionManager>();
            builder.RegisterType<StandardPathFinder>().As<IPathFinder>();
            StubGraphManager.Setup(builder);

            builder.Update(_container);


            _graphDescriptor = _container.Resolve<IGraphManager>().FindGraph(null);
            _contentManager = _container.Resolve<IContentManager>();
        }

        [TestFixtureTearDown]
        public void Clean()
        {
        }

        [Test]
        public void TwoNotionSearchFindsSinglePath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["medicine"], nodes["colour"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(3));
            Assert.That(graph.VertexCount, Is.EqualTo(4));
            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["medicine"], nodes["cyanide"], nodes["cyan"], nodes["colour"] }), Is.True);
        }

        [Test]
        public void TwoNotionSearchFindsDualPath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["yellow"], nodes["light year"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(5));
            Assert.That(graph.VertexCount, Is.EqualTo(5));

            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] }), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] }), Is.True);
        }

        [Test]
        public void ThreeNotionSearchFindsDualPath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["power plant"], nodes["electricity"], nodes["blue"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(3));
            Assert.That(graph.VertexCount, Is.EqualTo(4));

            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["power plant"], nodes["hydroelectric power plant"], nodes["blue"] }), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["power plant"], nodes["hydroelectric power plant"], nodes["electricity"] }), Is.True);
        }

        [Test]
        public void FourNotionSearchFindsSinglePath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["power plant"], nodes["electricity"], nodes["blue"], nodes["hydroelectric power plant"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(4));
            Assert.That(graph.VertexCount, Is.EqualTo(4));

            Assert.That(PathVerifier.PathExistsInGraph(graph, new IContent[] { nodes["blue"], nodes["hydroelectric power plant"], nodes["power plant"], nodes["electricity"] }), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["blue"], nodes["medicine"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(0));
            Assert.That(graph.VertexCount, Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, _graphDescriptor).Nodes;

            var graph = _graphDescriptor.Services.Mind.MakeAssociations(new IContent[] { nodes["writer"], nodes["plant"] }, new MindSettings { Algorithm = "sophisticated" }).ToGraph();

            Assert.That(graph.EdgeCount, Is.EqualTo(0));
            Assert.That(graph.VertexCount, Is.EqualTo(0));
        }
    }
}
