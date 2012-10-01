using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Associativy.Tests.Stubs;
using Autofac;
using Moq;
using NHibernate;
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

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class StandardPathFinderTests : ContentManagerEnabledTestBase
    {
        private IPathFinder _pathFinder;

        [SetUp]
        public override void Init()
        {
            base.Init();

            var builder = new ContainerBuilder();


            builder.RegisterInstance(new GraphEventMonitor(new Signals(), new SignalStorage())).As<IGraphEventMonitor>();
            builder.RegisterInstance(new StubGraphManager()).As<IGraphManager>();
            builder.RegisterInstance(new StubGraphEditor()).As<IGraphEditor>();
            builder.RegisterType<StandardPathFinder>().As<IPathFinder>();


            builder.Update(_container);

            _pathFinder = _container.Resolve<IPathFinder>();
        }

        [TestFixtureTearDown]
        public void Clean()
        {
        }

        [Test]
        public void SinglePathsAreFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var result = CalcPathResult(nodes["medicine"], nodes["colour"]);
            var succeededGraph = result.SucceededGraph;
            var succeededPaths = result.SucceededPaths;

            var rightPath = new IContent[] { nodes["medicine"], nodes["cyanide"], nodes["cyan"], nodes["colour"] };

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(4));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(3));

            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath), Is.True);

            Assert.That(succeededPaths.Count(), Is.EqualTo(1));
            Assert.That(PathVerifier.VerifyPath(succeededPaths.First(), rightPath), Is.True);
        }

        [Test]
        public void SinglePathsAreFound2()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var result = CalcPathResult(nodes["American"], nodes["writer"]);
            var succeededGraph = result.SucceededGraph;
            var succeededPaths = result.SucceededPaths;

            var rightPath = new IContent[] { nodes["American"], nodes["Ernest Hemingway"], nodes["writer"] };

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(3));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(2));

            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath), Is.True);

            Assert.That(succeededPaths.Count(), Is.EqualTo(1));
            Assert.That(PathVerifier.VerifyPath(succeededPaths.First(), rightPath), Is.True);
        }

        [Test]
        public void DualPathsAreFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var result = CalcPathResult(nodes["yellow"], nodes["light year"]);
            var succeededGraph = result.SucceededGraph;
            var succeededPaths = result.SucceededPaths.ToList();

            var rightPath1 = new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] };
            var rightPath2 = new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] };

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(5));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(5));

            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath1), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath2), Is.True);

            Assert.That(succeededPaths.Count, Is.EqualTo(2));

            Assert.That(PathVerifier.VerifyPath(succeededPaths[0], rightPath1), Is.True);
            Assert.That(PathVerifier.VerifyPath(succeededPaths[1], rightPath2), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var result = CalcPathResult(nodes["blue"], nodes["medicine"]);
            var succeededGraph = result.SucceededGraph;
            var succeededPaths = result.SucceededPaths;

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));

            Assert.That(succeededPaths.Count(), Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var result = CalcPathResult(nodes["writer"], nodes["plant"]);
            var succeededGraph = result.SucceededGraph;
            var succeededPaths = result.SucceededPaths;

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));

            Assert.That(succeededPaths.Count(), Is.EqualTo(0));
        }

        public PathResult CalcPathResult(IContent node1, IContent node2)
        {
            return _pathFinder.FindPaths(TestGraphHelper.TestGraphContext(), node1.Id, node2.Id);
        }
    }
}
