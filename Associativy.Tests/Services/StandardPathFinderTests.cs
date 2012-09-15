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

            var succeededPaths = CalcSucceededPaths(nodes["medicine"], nodes["colour"]);

            Assert.That(succeededPaths.Count(), Is.EqualTo(1));
            Assert.That(VerifyPath(succeededPaths.First(), new IContent[] { nodes["medicine"], nodes["cyanide"], nodes["cyan"], nodes["colour"] }), Is.True);
        }

        [Test]
        public void DualPathsAreFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            var succeededPaths = CalcSucceededPaths(nodes["yellow"], nodes["light year"]).ToList();

            Assert.That(succeededPaths.Count, Is.EqualTo(2));

            Assert.That(VerifyPath(succeededPaths[0], new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] }), Is.True);
            Assert.That(VerifyPath(succeededPaths[1], new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] }), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            Assert.That(CalcSucceededPaths(nodes["blue"], nodes["medicine"]).Count(), Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_container).Nodes;

            Assert.That(CalcSucceededPaths(nodes["writer"], nodes["plant"]).Count(), Is.EqualTo(0));
        }

        public IEnumerable<IEnumerable<int>> CalcSucceededPaths(IContent node1, IContent node2)
        {
            return _pathFinder.FindPaths(TestGraphHelper.TestGraphContext(), node1.Id, node2.Id).SucceededPaths;
        }

        public static bool VerifyPath(IEnumerable<int> path, IEnumerable<IContent> verification)
        {
            var pathList = path.ToList();
            var verificationList = verification.ToList();

            var i = 0;
            while (i < pathList.Count && verificationList[i].Id == pathList[i])
            {
                i++;   
            }

            return i == pathList.Count;
        }
    }
}
