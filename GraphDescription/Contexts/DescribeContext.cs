using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public abstract class DescribeContext
    {
        public abstract DescribeFor For(string graphName);
        public abstract GraphDescriptor Describe(IGraphContext graphContext);
        public abstract IEnumerable<GraphDescriptor> Describe(IContentContext contentContext);
    }
}