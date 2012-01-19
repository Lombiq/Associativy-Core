using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices : IAssociativyServices
    {
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
            IGraphService graphService,
            IMind mind,
            INodeManager nodeManager)
        {
            _graphService = graphService;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}