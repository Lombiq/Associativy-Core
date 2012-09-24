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


        public PathServices(
            IConnectionManager connectionManager,
            IPathFinder pathFinder)
        {
            _connectionManager = connectionManager;
            _pathFinder = pathFinder;
        }
    }
}