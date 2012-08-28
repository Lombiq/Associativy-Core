using Associativy.GraphDiscovery;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices : IAssociativyServices
    {
        protected readonly IGraphManager _graphManager;
        public IGraphManager GraphManager
        {
            get { return _graphManager; }
        }

        protected readonly IGraphEditor _graphEditor;
        public IGraphEditor GraphEditor
        {
            get { return _graphEditor; }
        }

        protected readonly IMind _mind;
        public IMind Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager _nodeManager;
        public INodeManager NodeManager
        {
            get { return _nodeManager; }
        }


        public AssociativyServices(
            IGraphManager graphManager,
            IGraphEditor graphEditor,
            IMind mind,
            INodeManager nodeManager)
        {
            _graphManager = graphManager;
            _graphEditor = graphEditor;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}