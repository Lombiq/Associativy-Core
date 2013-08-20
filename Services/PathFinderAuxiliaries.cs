using System.Collections.Generic;
using Associativy.Queryable;
using QuickGraph;

namespace Associativy.Services
{
    public class PathFinderAuxiliaries : IPathFinderAuxiliaries
    {
        protected readonly IGraphEditor _graphEditor;
        private readonly IGraphCacheService _cacheService;
        private readonly IQueryableGraphFactory _queryableFactory;

        public IGraphEditor GraphEditor { get { return _graphEditor; } }
        public IGraphCacheService CacheService { get { return _cacheService; } }
        public IQueryableGraphFactory QueryableFactory { get { return _queryableFactory; } }


        public PathFinderAuxiliaries(
            IGraphEditor graphEditor,
            IGraphCacheService cacheService,
            IQueryableGraphFactory queryableFactory)
        {
            _graphEditor = graphEditor;
            _cacheService = cacheService;
            _queryableFactory = queryableFactory;
        }


        public IUndirectedGraph<int, IUndirectedEdge<int>> PathToGraph(IEnumerable<IList<int>> succeededPaths)
        {
            var g = _graphEditor.GraphFactory<int>();

            foreach (var path in succeededPaths)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    g.AddVerticesAndEdge(new UndirectedEdge<int>(path[i - 1], path[i]));
                }
            }

            return g;
        }


        public class PathResult : IPathResult
        {
            public IQueryableGraph<int> SucceededGraph { get; set; }
            public IEnumerable<IEnumerable<int>> SucceededPaths { get; set; }
        }
    }
}