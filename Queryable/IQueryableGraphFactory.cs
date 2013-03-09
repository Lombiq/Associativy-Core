using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Associativy.Queryable
{
    public interface IQueryableGraphFactory : IDependency
    {
        IQueryableGraph<TNode> Factory<TNode>(ValueFactory<TNode> graphFactory);
    }


    public static class QueryableGraphFactoryExtensions
    {
        public static IQueryableGraph<TNode> CopyValueFactory<TNode>(this IQueryableGraphFactory factory, IQueryableGraph<TNode> queryableGraph)
        {
            return factory.Factory(queryableGraph.ValueFactory);
        }
    }
}