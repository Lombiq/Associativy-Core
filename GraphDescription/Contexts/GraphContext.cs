using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public class GraphContext : IGraphContext
    {
        private string _graphName;
        public string GraphName
        {
            get { return _graphName; }
        }

        public GraphContext(string graphName)
        {
            _graphName = graphName;
        }
    }
}