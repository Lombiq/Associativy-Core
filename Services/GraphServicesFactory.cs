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

    public interface IGraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager, TGraphStatisticsService> : IGraphServicesFactory
        where TMind : IMind
        where TConnectionManager : IConnectionManager
        where TPathFinder : IPathFinder
        where TNodeManager : INodeManager
        where TGraphStatisticsService : IGraphStatisticsService
    {
    }

    [OrchardFeature("Associativy")]
    public class GraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager, TGraphStatisticsService>
        : IGraphServicesFactory<TMind, TConnectionManager, TPathFinder, TNodeManager, TGraphStatisticsService>
        where TMind : IMind
        where TConnectionManager : IConnectionManager
        where TPathFinder : IPathFinder
        where TNodeManager : INodeManager
        where TGraphStatisticsService : IGraphStatisticsService
    {
        private readonly Func<IGraphDescriptor, TMind> _mindFactory;
        private readonly Func<IGraphDescriptor, TConnectionManager> _connectionManagerFactory;
        private readonly Func<IGraphDescriptor, TPathFinder> _pathFinderFactory;
        private readonly Func<IGraphDescriptor, TNodeManager> _nodeManagerFactory;
        private readonly Func<IGraphDescriptor, TGraphStatisticsService> _graphStatisticsServiceFactory;


        public GraphServicesFactory(
            Func<IGraphDescriptor, TMind> mindFactory,
            Func<IGraphDescriptor, TConnectionManager> connectionManagerFactory,
            Func<IGraphDescriptor, TPathFinder> pathFinderFactory,
            Func<IGraphDescriptor, TNodeManager> nodeManagerFactory,
            Func<IGraphDescriptor, TGraphStatisticsService> graphStatisticsServiceFactory)
        {
            _mindFactory = mindFactory;
            _connectionManagerFactory = connectionManagerFactory;
            _pathFinderFactory = pathFinderFactory;
            _nodeManagerFactory = nodeManagerFactory;
            _graphStatisticsServiceFactory = graphStatisticsServiceFactory;
        }


        public IGraphServices Factory(IGraphDescriptor graphDescriptor)
        {
            return new GraphServices(
                _mindFactory(graphDescriptor),
                _connectionManagerFactory(graphDescriptor),
                _pathFinderFactory(graphDescriptor),
                _nodeManagerFactory(graphDescriptor),
                _graphStatisticsServiceFactory(graphDescriptor)
                );
        }
    }
}