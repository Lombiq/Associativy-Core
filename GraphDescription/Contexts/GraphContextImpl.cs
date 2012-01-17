using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public class GraphContextImpl : GraphContext
    {
        private string _graphName;
        public override string GraphName
        {
            get { return _graphName; }
        }

        public GraphContextImpl(string technicalGraphName)
        {
            _graphName = technicalGraphName;
        }
    }
}