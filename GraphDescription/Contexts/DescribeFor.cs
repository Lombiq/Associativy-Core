using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Services;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public abstract class DescribeFor
    {
        public abstract void Set(string[] contentTypes, IConnectionManager connectionManager);
    }
}