using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

// If circular dependencies should happen, use property injection:
// http://www.szmyd.com.pl/blog/di-property-injection-in-orchard
// http://code.google.com/p/autofac/wiki/CircularDependencies

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IAssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly IConnectionManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> _connectionManager;
        public IConnectionManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> ConnectionManager
        {
            get { return _connectionManager; }
        }

        private readonly IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> _mind;
        public IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> Mind
        {
            get { return _mind; }
        }

        private readonly INodeManager<TNodePart, TNodePartRecord, TNodeParams> _nodeManager;
        public INodeManager<TNodePart, TNodePartRecord, TNodeParams> NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            IConnectionManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> connectionManager,
            IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> mind,
            INodeManager<TNodePart, TNodePartRecord, TNodeParams> nodeManager)
        {
            _connectionManager = connectionManager;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}