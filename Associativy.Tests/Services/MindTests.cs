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
using Associativy.Models.Mind;
using Associativy.Tests.Helpers;

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class MindTests : ContentManagerEnabledTestBase, IMindEventHandler
    {
        private IMind _mind;
        private IUndirectedGraph<int, IUndirectedEdge<int>> _currentIdGraph;

        [SetUp]
        public override void Init()
        {
            base.Init();

            var builder = new ContainerBuilder();


            builder.RegisterInstance(new GraphEventMonitor(new Signals(), new SignalStorage())).As<IGraphEventMonitor>();
            builder.RegisterInstance(new StubGraphManager()).As<IGraphManager>();
            builder.RegisterInstance(new StubNodeManager()).As<INodeManager>();
            builder.RegisterInstance(new StubGraphEditor()).As<IGraphEditor>();
            builder.RegisterInstance(this).As<IMindEventHandler>();
            builder.RegisterType<Mind>().As<IMind>();


            builder.Update(_container);

            _mind = _container.Resolve<IMind>();
        }

        [TestFixtureTearDown]
        public void Clean()
        {
        }

        [Test]
        public void TwoNotionSearchFindsSinglePath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["medicine"], nodes["colour"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(3));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(4));
            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["medicine"], nodes["cyanide"], nodes["cyan"], nodes["colour"] }), Is.True);
        }

        [Test]
        public void TwoNotionSearchFindsDualPath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["yellow"], nodes["light year"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(5));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(5));

            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] }), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] }), Is.True);
        }

        [Test]
        public void ThreeNotionSearchFindsDualPath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["power plant"], nodes["electricity"], nodes["blue"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(3));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(4));

            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["power plant"], nodes["hydroelectric power plant"], nodes["blue"] }), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["power plant"], nodes["hydroelectric power plant"], nodes["electricity"] }), Is.True);
        }

        [Test]
        public void FourNotionSearchFindsSinglePath()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["power plant"], nodes["electricity"], nodes["blue"], nodes["hydroelectric power plant"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(4));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(4));

            Assert.That(PathVerifier.PathExistsInGraph(_currentIdGraph, new IContent[] { nodes["blue"], nodes["hydroelectric power plant"], nodes["power plant"], nodes["electricity"] }), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["blue"], nodes["medicine"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(0));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            _mind.MakeAssociations(TestGraphHelper.TestGraphContext(), new IContent[] { nodes["writer"], nodes["plant"] }, new MindSettings { Algorithm = "sophisticated" });

            Assert.That(_currentIdGraph.EdgeCount, Is.EqualTo(0));
            Assert.That(_currentIdGraph.VertexCount, Is.EqualTo(0));
        }

        public void BeforeWholeContentGraphBuilding(IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            _currentIdGraph = idGraph;
        }

        public void BeforePartialContentGraphBuilding(IGraphContext graphContext, IContent centerNode, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            _currentIdGraph = idGraph;
        }

        public void BeforeSearchedContentGraphBuilding(IGraphContext graphContext, IEnumerable<IContent> nodes, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            _currentIdGraph = idGraph;
        }
    }
}
