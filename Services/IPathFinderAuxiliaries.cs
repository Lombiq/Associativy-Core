using System.Collections.Generic;
using Associativy.Queryable;
using Orchard;
using QuickGraph;

namespace Associativy.Services
{
    public interface IPathFinderAuxiliaries : IDependency
    {
        IGraphEditor GraphEditor { get; }
        IGraphCacheService CacheService { get; }
        IQueryableGraphFactory QueryableFactory { get; }

        IUndirectedGraph<int, IUndirectedEdge<int>> PathToGraph(IEnumerable<IList<int>> succeededPaths);
    }
}
