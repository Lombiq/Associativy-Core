using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Associativy.Tests.Stubs;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.Tests.Stubs;
using Orchard.Tests.Utility;

namespace Associativy.Tests.Services
{
    [TestFixture]
    public class MemoryConnectionManagerTests
    {
        private IContainer _container;
        private IMemoryConnectionManager _memoryConnectionManager;

        [SetUp]
        public void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAutoMocking(MockBehavior.Loose);
            builder.RegisterInstance(new StubGraphDescriptor()).As<IGraphDescriptor>();
            builder.RegisterInstance(new StubCacheManager()).As<ICacheManager>();
            builder.RegisterType<MemoryConnectionManager>().As<IMemoryConnectionManager>();

            _container = builder.Build();

            _memoryConnectionManager = _container.Resolve<IMemoryConnectionManager>();

            SetupTestGraph();
        }

        [TestFixtureTearDown]
        public void Clean()
        {
        }

        [Test]
        public void ConnectionsShouldBePersisted()
        {
            Assert.That(_memoryConnectionManager.GetConnectionCount(), Is.EqualTo(6));

            var connections = _memoryConnectionManager.GetAll(0, int.MaxValue);

            Action<int, int> checkBothWays =
            (node1Id, node2Id) =>
            {
                Assert.That(connections.Any(connection => connection.Node1Id == node1Id && connection.Node2Id == node2Id || connection.Node1Id == node2Id && connection.Node2Id == node1Id));
            };

            Assert.That(connections.Count(), Is.EqualTo(6));
            checkBothWays(1, 2);
            checkBothWays(1, 3);
            checkBothWays(1, 5);
            checkBothWays(2, 4);
            checkBothWays(2, 5);
            checkBothWays(3, 5);
        }

        [Test]
        public void NeighbourCountsAreValid()
        {
            Assert.That(_memoryConnectionManager.GetNeighbourCount(1), Is.EqualTo(3));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(2), Is.EqualTo(3));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(3), Is.EqualTo(2));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(4), Is.EqualTo(1));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(5), Is.EqualTo(3));
        }

        [Test]
        public void AllNeighboursCanBeDeleted()
        {
            _memoryConnectionManager.DeleteFromNode(1);

            Assert.That(_memoryConnectionManager.GetConnectionCount(), Is.EqualTo(3));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(1), Is.EqualTo(0));
            Assert.That(_memoryConnectionManager.GetNeighbourIds(1).Count(), Is.EqualTo(0));
        }

        [Test]
        public void NeighboursAreProperlyFetched()
        {
            var neighbours = _memoryConnectionManager.GetNeighbourIds(2);

            Assert.That(neighbours.Count(), Is.EqualTo(3));
            Assert.That(neighbours.Contains(1));
            Assert.That(neighbours.Contains(4));
            Assert.That(neighbours.Contains(5));
        }

        [Test]
        public void DisconnectRemovesConnection()
        {
            _memoryConnectionManager.Disconnect(4, 2);

            Assert.That(!_memoryConnectionManager.AreNeighbours(2, 4));
            Assert.That(_memoryConnectionManager.GetNeighbourCount(4), Is.EqualTo(0));
            Assert.That(_memoryConnectionManager.GetNeighbourIds(4).Count(), Is.EqualTo(0));
        }


        private void SetupTestGraph()
        {
            _memoryConnectionManager.Connect(1, 2);
            _memoryConnectionManager.Connect(1, 3);
            _memoryConnectionManager.Connect(1, 5);
            _memoryConnectionManager.Connect(2, 4);
            _memoryConnectionManager.Connect(2, 5);
            _memoryConnectionManager.Connect(3, 5);
        }


        private class StubGraphDescriptor : IGraphDescriptor
        {
            public string Name
            {
                get { return "TestGraph"; }
            }

            public Orchard.Localization.LocalizedString DisplayName
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<string> ContentTypes
            {
                get { throw new NotImplementedException(); }
            }

            public IGraphServices Services
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
