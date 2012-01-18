using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Associativy.GraphDescription.Contexts;

namespace Associativy.GraphDescription
{
    public interface IGraphProvider : IDependency
    {
        void Describe(DescribeContext context);
    }
}
