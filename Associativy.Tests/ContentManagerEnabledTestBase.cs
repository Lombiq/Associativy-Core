using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Orchard.Tests;
using Orchard.ContentManagement.Records;
using Autofac;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Tests.UI.Navigation;
using Orchard.Core.Settings.Metadata;
using Orchard.Tests.Stubs;
using Orchard.ContentManagement.MetaData;
using Orchard;
using Orchard.FileSystems.AppData;
using Orchard.Caching;
using Orchard.Data;
using Moq;
using Orchard.Tests.ContentManagement;
using Orchard.Tests.Utility;
using NHibernate;

namespace Associativy.Tests
{
    public class ContentManagerEnabledTestBase
    {
        protected IContainer _container;

        protected ISessionFactory _sessionFactory;
        protected ISession _session;

        [TestFixtureSetUp]
        public virtual void InitFixture()
        {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(ContentTypeRecord),
                typeof(ContentItemRecord),
                typeof(ContentItemVersionRecord));
        }

        [SetUp]
        public virtual void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAutoMocking(MockBehavior.Loose);

            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
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

            _container = builder.Build();
        }
    }
}
