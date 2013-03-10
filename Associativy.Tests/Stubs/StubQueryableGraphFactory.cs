using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Queryable;

namespace Associativy.Tests.Stubs
{
    public class StubQueryableGraphFactory : IQueryableGraphFactory
    {
        public IQueryableGraph<TNode> Create<TNode>(ValueFactory<TNode> graphFactory)
        {
            return new QueryableGraph<TNode>(graphFactory);
        }
    }
}
