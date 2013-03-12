using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    public class GraphAwareServiceBase
    {
        protected readonly IGraphDescriptor _graphDescriptor;

        protected GraphAwareServiceBase(IGraphDescriptor graphDescriptor)
        {
            _graphDescriptor = graphDescriptor;
        }
    }
}