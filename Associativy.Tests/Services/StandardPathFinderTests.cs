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

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class StandardPathFinderTests
    {
        private IContainer _container;
        private IPathFinder _pathFinder;

        #region ContentManager setup
        private ISessionFactory _sessionFactory;
        private ISession _session;

        [TestFixtureSetUp]
        public void InitFixture()
        {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(ContentTypeRecord),
                typeof(ContentItemRecord),
                typeof(ContentItemVersionRecord));
        }
        #endregion

        [SetUp]
        public virtual void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAutoMocking(MockBehavior.Loose);


            builder.RegisterInstance(new GraphEventMonitor(new Signals(), new SignalStorage())).As<IGraphEventMonitor>();
            builder.RegisterInstance(new StubGraphManager()).As<IGraphManager>();
            builder.RegisterInstance(new StubGraphEditor()).As<IGraphEditor>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<StandardPathFinder>().As<IPathFinder>();

            #region ContentManager setup
            builder.RegisterType<DefaultContentManager>().As<IContentManager>();
            builder.RegisterType<ContentDefinitionManager>().As<IContentDefinitionManager>();
            builder.RegisterType<ContentDefinitionWriter>().As<IContentDefinitionWriter>();
            builder.RegisterType<StubOrchardServices>().As<IOrchardServices>();
            builder.RegisterType<StubAppDataFolder>().As<IAppDataFolder>();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterInstance(new Mock<ISettingsFormatter>().Object);
            _session = _sessionFactory.OpenSession();
            builder.RegisterInstance(new DefaultContentManagerTests.TestSessionLocator(_session)).As<ISessionLocator>();
            #endregion

            _container = builder.Build();

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

            var succeededGraph = CalcSucceededGraph(nodes["medicine"], nodes["colour"]);

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(4));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(3));

            Assert.That(PathExistsInGraph(succeededGraph, new IContent[] { nodes["medicine"], nodes["cyanide"], nodes["cyan"], nodes["colour"] }), Is.True);
        }

        [Test]
        public void DualPathsAreFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var succeededGraph = CalcSucceededGraph(nodes["yellow"], nodes["light year"]);

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(5));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(5));

            Assert.That(PathExistsInGraph(succeededGraph, new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] }), Is.True);
            Assert.That(PathExistsInGraph(succeededGraph, new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] }), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var succeededGraph = CalcSucceededGraph(nodes["blue"], nodes["medicine"]);

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var succeededGraph = CalcSucceededGraph(nodes["writer"], nodes["plant"]);

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));
        }

        public IUndirectedGraph<int, IUndirectedEdge<int>> CalcSucceededGraph(IContent node1, IContent node2)
        {
            return _pathFinder.FindPaths(TestGraphHelper.TestGraphContext(), node1.Id, node2.Id).SucceededGraph;
        }

        public static bool PathExistsInGraph(IUndirectedGraph<int, IUndirectedEdge<int>> graph, IEnumerable<IContent> path)
        {
            var pathList = path.ToList();
            if (!graph.ContainsVertex(pathList[0].Id)) return false;

            var nextIndex = 1;
            var node = pathList[0].Id;
            while (node != pathList.Last().Id)
            {
                var nextNode = pathList[nextIndex].Id;
                if (!graph.AdjacentEdges(node).Any(edge => edge.Target == nextNode)) return false;
                node = nextNode;
                nextIndex++;
            }

            return true;

            //var pathList = path.ToList();
            //var verificationList = verification.ToList();

            //var i = 0;
            //while (i < pathList.Count && verificationList[i].Id == pathList[i])
            //{
            //    i++;   
            //}

            //return i == pathList.Count;
        }
    }
}
