using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices: AssociativyServiceBase, IAssociativyServices
    {
        private object _graphDescriptorLocker = new object();
        public override IAssociativyGraphDescriptor GraphDescriptor
        {
            set
            {
                lock (_graphDescriptorLocker) // This is to ensure that used services also have the same graphDescriptor
                {
                    _graphService.GraphDescriptor = value;
                    _nodeManager.GraphDescriptor = value;
                    _mind.GraphDescriptor = value;
                    base.GraphDescriptor = value;
                }
            }
        }

        public IConnectionManager ConnectionManager
        {
            get { return GraphDescriptor.ConnectionManager; }
        }

        protected readonly IGraphService _graphService;
        public IGraphService GraphService
        {
            get { return _graphService; }
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
            IAssociativyGraphDescriptor associativyGraphDescriptor,
            IGraphService graphService,
            IMind mind,
            INodeManager nodeManager)
            : base(associativyGraphDescriptor)
        {
            _graphService = graphService;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}