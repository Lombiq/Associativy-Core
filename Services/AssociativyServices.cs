using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _mind;
        public IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> Mind
        {
            get { return _mind; }
        }

        private readonly INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _nodeManager;
        public INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> mind,
            INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> nodeManager)
        {
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}