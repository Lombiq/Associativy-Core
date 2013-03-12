using System;
using System.Collections.Generic;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [Serializable]
    public class GraphContext : IGraphContext
    {
        public string Name { get; set; }
        public IEnumerable<string> ContentTypes { get; set; }

        private static readonly GraphContext _empty = new GraphContext();
        public static GraphContext Empty { get { return _empty; } }
    }
}