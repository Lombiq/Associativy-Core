using System;
using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public abstract class GraphServicesBase : IGraphServices
    {
        protected IMind _mind;
        public IMind Mind { get { return _mind; } }

        protected IConnectionManager _connectionManager;
        public IConnectionManager ConnectionManager { get { return _connectionManager; } }

        protected IPathFinder _pathFinder;
        public IPathFinder PathFinder { get { return _pathFinder; } }

        protected INodeManager _nodeManager;
        public INodeManager NodeManager { get { return _nodeManager; } }

        protected IGraphStatisticsService _graphStatisticsService;
        public IGraphStatisticsService GraphStatisticsService { get { return _graphStatisticsService; } }
    }

    [OrchardFeature("Associativy")]
    public class GraphServices : GraphServicesBase
    {
        public GraphServices(
            IMind mind,
            IConnectionManager connectionManager,
            IPathFinder pathFinder,
            INodeManager nodeManager,
            IGraphStatisticsService graphStatisticsService)
        {
            _mind = mind;
            _connectionManager = connectionManager;
            _pathFinder = pathFinder;
            _nodeManager = nodeManager;
            _graphStatisticsService = graphStatisticsService;
        }
    }
}