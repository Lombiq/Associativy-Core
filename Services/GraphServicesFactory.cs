using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    public interface IGraphServicesFactory
    {
        IGraphServices Factory(IGraphDescriptor graphDescriptor);
    }

    public interface IGraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager> : IGraphServicesFactory
        where TMind : IMind
        where TConnectionManager : IConnectionManager
        where TPathFinder : IPathFinder
        where TNodeManager : INodeManager
    {
    }

    [OrchardFeature("Associativy")]
    public class GraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager>
        : IGraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager>
        where TMind : IMind
        where TConnectionManager : IConnectionManager
        where TPathFinder : IPathFinder
        where TNodeManager : INodeManager
    {
        private readonly Func<IGraphDescriptor, TMind> _mindFactory;
        private readonly Func<IGraphDescriptor, TConnectionManager> _connectionManagerFactory;
        private readonly Func<IGraphDescriptor, TPathFinder> _pathFinderFactory;
        private readonly Func<IGraphDescriptor, TNodeManager> _nodeManagerFactory;


        public GraphServicesFactory(
            Func<IGraphDescriptor, TMind> mindFactory,
            Func<IGraphDescriptor, TConnectionManager> connectionManagerFactory,
            Func<IGraphDescriptor, TPathFinder> pathFinderFactory,
            Func<IGraphDescriptor, TNodeManager> nodeManagerFactory)
        {
            _mindFactory = mindFactory;
            _connectionManagerFactory = connectionManagerFactory;
            _pathFinderFactory = pathFinderFactory;
            _nodeManagerFactory = nodeManagerFactory;
        }


        public IGraphServices Factory(IGraphDescriptor graphDescriptor)
        {
            return new GraphServices(
                _mindFactory(graphDescriptor),
                _connectionManagerFactory(graphDescriptor),
                _pathFinderFactory(graphDescriptor),
                _nodeManagerFactory(graphDescriptor)
                );
        }
    }
}