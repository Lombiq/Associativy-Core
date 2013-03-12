using Orchard;

namespace Associativy.Queryable
{
    /// <summary>
    /// Creates IQueryableGraph instances
    /// </summary>
    public interface IQueryableGraphFactory : IDependency
    {
        IQueryableGraph<TNode> Create<TNode>(ValueFactory<TNode> valueFactory);
    }


    public static class QueryableGraphFactoryExtensions
    {
        public static IQueryableGraph<TNode> CopyValueFactory<TNode>(this IQueryableGraphFactory factory, IQueryableGraph<TNode> queryableGraph)
        {
            return factory.Create(queryableGraph.ValueFactory);
        }
    }
}