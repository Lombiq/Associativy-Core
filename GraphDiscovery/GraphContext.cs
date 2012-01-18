using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphContext : IGraphContext
    {
        private string _providerName;
        public string ProviderName
        {
            get { return _providerName; }
        }

        public GraphContext(string providerName)
        {
            _providerName = providerName;
        }
    }
}