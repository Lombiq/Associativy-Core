using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

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