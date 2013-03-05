using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class GraphServiceBase
    {
        protected readonly IGraphDescriptor _graphDescriptor;

        protected GraphServiceBase(IGraphDescriptor graphDescriptor)
        {
            _graphDescriptor = graphDescriptor;
        }
    }
}