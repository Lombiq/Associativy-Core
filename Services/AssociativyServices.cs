using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TAssociativyGraphDescriptor> : AssociativyServiceBase, IAssociativyServices<TAssociativyGraphDescriptor>
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
        private readonly object _graphDescriptorLocker = new object();
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

        protected readonly IGraphService<TAssociativyGraphDescriptor> _graphService;
        public IGraphService<TAssociativyGraphDescriptor> GraphService
        {
            get { return _graphService; }
        }

        protected readonly IMind<TAssociativyGraphDescriptor> _mind;
        public IMind<TAssociativyGraphDescriptor> Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager<TAssociativyGraphDescriptor> _nodeManager;
        public INodeManager<TAssociativyGraphDescriptor> NodeManager
        {
            get { return _nodeManager; }
        }


        public AssociativyServices(
            TAssociativyGraphDescriptor associativyGraphDescriptor,
            IGraphService<TAssociativyGraphDescriptor> graphService,
            IMind<TAssociativyGraphDescriptor> mind,
            INodeManager<TAssociativyGraphDescriptor> nodeManager)
            : base(associativyGraphDescriptor)
        {
            _graphService = graphService;
            _nodeManager = nodeManager;
            _mind = mind;
        }

    }
    [OrchardFeature("Associativy")]
    public class AssociativyServices : AssociativyServices<IAssociativyGraphDescriptor>, IAssociativyServices
    {
        public AssociativyServices(
            IAssociativyGraphDescriptor associativyGraphDescriptor,
            IGraphService graphService,
            IMind mind,
            INodeManager nodeManager)
            : base(associativyGraphDescriptor, graphService, mind, nodeManager)
        {
        }

        new public IGraphService GraphService
        {
            get { return (IGraphService)_graphService; }
        }

        new public IMind Mind
        {
            get { return (IMind)_mind; }
        }

        new public INodeManager NodeManager
        {
            get { return (INodeManager)_nodeManager; }
        }
    }
}