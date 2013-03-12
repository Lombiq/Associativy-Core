using System;
using Orchard;

namespace Associativy.Queryable
{
    public class QueryableGraphFactory : IQueryableGraphFactory
    {
        private readonly IWorkContextAccessor _wca;


        public QueryableGraphFactory(IWorkContextAccessor wca)
        {
            _wca = wca;
        }


        public IQueryableGraph<TNode> Create<TNode>(ValueFactory<TNode> valueFactory)
        {
            var factory = _wca.GetContext().Resolve<Func<ValueFactory<TNode>, IQueryableGraph<TNode>>>();
            return factory(valueFactory);
        }
    }
}