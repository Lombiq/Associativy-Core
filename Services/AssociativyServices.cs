using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

// If circular dependencies should happen, use property injection:
// http://www.szmyd.com.pl/blog/di-property-injection-in-orchard
// http://code.google.com/p/autofac/wiki/CircularDependencies

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> connectionManager;
        public IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> ConnectionManager
        {
            get { return connectionManager; }
        }

        protected readonly IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> mind;
        public IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> Mind
        {
            get { return mind; }
        }

        protected readonly INodeManager<TNodePart, TNodePartRecord> nodeManager;
        public INodeManager<TNodePart, TNodePartRecord> NodeManager
        {
            get { return nodeManager; }
        }

        public AssociativyServices(
            IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> connectionManager,
            IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> mind,
            INodeManager<TNodePart, TNodePartRecord> nodeManager)
        {
            this.connectionManager = connectionManager;
            this.nodeManager = nodeManager;
            this.mind = mind;
        }
    }
}