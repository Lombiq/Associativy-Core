using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class GraphAwareServiceBase
    {
        protected readonly IGraphDescriptor _graphDescriptor;

        protected GraphAwareServiceBase(IGraphDescriptor graphDescriptor)
        {
            _graphDescriptor = graphDescriptor;
        }
    }
}