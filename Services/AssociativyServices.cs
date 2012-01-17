using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TGraphDescriptor> : AssociativyServiceBase, IAssociativyServices<TGraphDescriptor>
        where TGraphDescriptor : IGraphDescriptor
    {
        private readonly object _graphDescriptorLocker = new object();
        public override IGraphDescriptor GraphDescriptor
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

        protected readonly IGraphService<TGraphDescriptor> _graphService;
        public IGraphService<TGraphDescriptor> GraphService
        {
            get { return _graphService; }
        }

        protected readonly IMind<TGraphDescriptor> _mind;
        public IMind<TGraphDescriptor> Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager<TGraphDescriptor> _nodeManager;
        public INodeManager<TGraphDescriptor> NodeManager
        {
            get { return _nodeManager; }
        }


        public AssociativyServices(
            TGraphDescriptor graphDescriptor,
            IGraphService<TGraphDescriptor> graphService,
            IMind<TGraphDescriptor> mind,
            INodeManager<TGraphDescriptor> nodeManager)
            : base(graphDescriptor)
        {
            _graphService = graphService;
            _nodeManager = nodeManager;
            _mind = mind;
        }

    }
    [OrchardFeature("Associativy")]
    public class AssociativyServices : AssociativyServices<IGraphDescriptor>, IAssociativyServices
    {
        public AssociativyServices(
            IGraphDescriptor graphDescriptor,
            IGraphService graphService,
            IMind mind,
            INodeManager nodeManager)
            : base(graphDescriptor, graphService, mind, nodeManager)
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