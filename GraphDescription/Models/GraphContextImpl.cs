using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Models
{
    [OrchardFeature("Associativy")]
    public class GraphContextImpl : GraphContext
    {
        private string _technicalGraphName;
        public override string TechnicalGraphName
        {
            get { return _technicalGraphName; }
        }

        public GraphContextImpl(string technicalGraphName)
        {
            _technicalGraphName = technicalGraphName;
        }
    }
}