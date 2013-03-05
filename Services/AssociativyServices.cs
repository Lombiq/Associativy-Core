using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices : IAssociativyServices
    {
        protected readonly IGraphManager _graphManager;
        public IGraphManager GraphManager { get { return _graphManager; } }

        protected readonly IGraphEditor _graphEditor;
        public IGraphEditor GraphEditor { get { return _graphEditor; } }


        public AssociativyServices(
            IGraphManager graphManager,
            IGraphEditor graphEditor)
        {
            _graphManager = graphManager;
            _graphEditor = graphEditor;
        }
    }
}