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
using Orchard.Caching.Services;
using Associativy.Queryable;

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class StandardPathFinderTests : PathFinderTestsBase
    {
        [SetUp]
        public override void Init()
        {
            base.Init();

            var builder = new ContainerBuilder();

            builder.RegisterType<MemoryConnectionManager>().As<IConnectionManager>();
            builder.RegisterType<StandardPathFinder>().As<IPathFinder>();

            builder.Update(_container);
        }
    }
}
