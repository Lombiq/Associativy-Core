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
        public string GraphName { get; set; }
        public IEnumerable<string> ContentTypes { get; set; }
    }
}