using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription.Models
{
    [OrchardFeature("Associativy")]
    public abstract class GraphContext
    {
        public abstract string TechnicalGraphName { get; }
    }
}