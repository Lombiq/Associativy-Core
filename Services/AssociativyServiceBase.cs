using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public abstract class AssociativyServiceBase
    {
        protected readonly IGraphManager _graphManager;

        protected AssociativyServiceBase(IGraphManager graphManager)
        {
            _graphManager = graphManager;
        }
    }
}