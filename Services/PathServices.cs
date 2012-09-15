using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class PathServices : IPathServices
    {
        protected readonly IConnectionManager _connectionManager;
        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
        }

        protected readonly IPathFinder _pathFinder;
        public IPathFinder PathFinder
        {
            get { return _pathFinder; }
        }

        protected readonly IMind _mind;
        public IMind Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager _nodeManager;
        public INodeManager NodeManager
        {
            get { return _nodeManager; }
        }


        public PathServices(
            IConnectionManager connectionManager,
            IPathFinder pathFinder)
        {
            _connectionManager = connectionManager;
            _pathFinder = pathFinder;
        }
    }
}