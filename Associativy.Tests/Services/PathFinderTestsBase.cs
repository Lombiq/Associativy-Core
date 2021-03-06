﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;
using Associativy.Queryable;
using Associativy.Services;
using Associativy.Tests.Helpers;
using Associativy.Tests.Stubs;
using Autofac;
using NUnit.Framework;
using Orchard.Caching.Services;
using Orchard.ContentManagement;

namespace Associativy.Tests.Services
{
    public abstract class PathFinderTestsBase : ContentManagerEnabledTestBase
    {
        protected Lazy<IGraphDescriptor> _graphDescriptorLazy;
        protected IContentManager _contentManager;

        public IGraphDescriptor GraphDescriptor
        {
            get { return _graphDescriptorLazy.Value; }
        }
        

        [SetUp]
        public override void Init()
        {
            base.Init();

            var builder = new ContainerBuilder();


            var cacheService = new StubCacheService();
            builder.RegisterInstance(new GraphEventMonitor(cacheService)).As<IGraphEventMonitor>();
            builder.RegisterInstance(new StubGraphEditor()).As<IGraphEditor>();
            builder.RegisterInstance(cacheService).As<ICacheService>();
            builder.RegisterInstance(new StubQueryableGraphFactory()).As<IQueryableGraphFactory>();
            builder.RegisterType<GraphCacheService>().As<IGraphCacheService>();
            builder.RegisterType<StubGraphManager>().As<IGraphManager>();
            builder.RegisterType<PathFinderAuxiliaries>().As<IPathFinderAuxiliaries>();

            StubGraphManager.Setup(builder);

            builder.Update(_container);

            _graphDescriptorLazy = new Lazy<IGraphDescriptor>(() => _container.Resolve<IGraphManager>().FindGraph(null));
            _contentManager = _container.Resolve<IContentManager>();
        }

        [TestFixtureTearDown]
        public virtual void Clean()
        {
        }

        [Test]
        public void SinglePathsAreFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, GraphDescriptor).Nodes;

            var result = CalcPathResult(nodes["medicine"], nodes["colour"]);
            var succeededGraph = result.SucceededGraph.ToGraph();
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
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, GraphDescriptor).Nodes;

            var result = CalcPathResult(nodes["American"], nodes["writer"]);
            var succeededGraph = result.SucceededGraph.ToGraph();
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
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, GraphDescriptor).Nodes;

            var result = CalcPathResult(nodes["yellow"], nodes["light year"]);
            var succeededGraph = result.SucceededGraph.ToGraph();
            var succeededPaths = result.SucceededPaths.ToList();

            var rightPath1 = new IContent[] { nodes["yellow"], nodes["sun"], nodes["light"], nodes["light year"] };
            var rightPath2 = new IContent[] { nodes["yellow"], nodes["colour"], nodes["light"], nodes["light year"] };

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(5));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(5));

            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath1), Is.True);
            Assert.That(PathVerifier.PathExistsInGraph(succeededGraph, rightPath2), Is.True);

            Assert.That(succeededPaths.Count, Is.EqualTo(2));


            // The order of found paths is not fixed.
            Assert.That(PathVerifier.VerifyPath(succeededPaths[0], rightPath1) || PathVerifier.VerifyPath(succeededPaths[0], rightPath2), Is.True);
            Assert.That(PathVerifier.VerifyPath(succeededPaths[1], rightPath1) || PathVerifier.VerifyPath(succeededPaths[1], rightPath2), Is.True);
        }

        [Test]
        public void TooLongPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, GraphDescriptor).Nodes;

            var result = CalcPathResult(nodes["blue"], nodes["medicine"]);
            var succeededGraph = result.SucceededGraph.ToGraph();
            var succeededPaths = result.SucceededPaths;

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));

            Assert.That(succeededPaths.Count(), Is.EqualTo(0));
        }

        [Test]
        public void NotConnectedPathsAreNotFound()
        {
            var nodes = TestGraphHelper.BuildTestGraph(_contentManager, GraphDescriptor).Nodes;

            var result = CalcPathResult(nodes["writer"], nodes["plant"]);
            var succeededGraph = result.SucceededGraph.ToGraph();
            var succeededPaths = result.SucceededPaths;

            Assert.That(succeededGraph.VertexCount, Is.EqualTo(0));
            Assert.That(succeededGraph.EdgeCount, Is.EqualTo(0));

            Assert.That(succeededPaths.Count(), Is.EqualTo(0));
        }

        protected IPathResult CalcPathResult(IContent node1, IContent node2)
        {
            return GraphDescriptor.Services.PathFinder.FindPaths(node1.Id, node2.Id, PathFinderSettings.Default);
        }
    }
}
