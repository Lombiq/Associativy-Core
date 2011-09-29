using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IAssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> _mind;
        public IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> Mind
        {
            get { return _mind; }
        }

        private readonly INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> _nodeManager;
        public INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> mind,
            INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> nodeManager)
        {
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}