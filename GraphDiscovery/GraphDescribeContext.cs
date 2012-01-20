using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class GraphDescribeContext
    {
        public abstract void As(string graphName, LocalizedString displayGraphName, IEnumerable<string> contentTypes, IConnectionManager connectionManager);
    }
}