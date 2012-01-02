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
    public class AssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext>
        : AssociativyService<TAssociativyContext>, IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        public IAssociativyContext Context
        {
            get { return _associativyContext; }
        }

        protected readonly IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> _connectionManager;
        public IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> ConnectionManager
        {
            get { return _connectionManager; }
        }

        protected readonly IMind<TNodeToNodeConnectorRecord, TAssociativyContext> _mind;
        public IMind<TNodeToNodeConnectorRecord, TAssociativyContext> Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager<TAssociativyContext> _nodeManager;
        public INodeManager<TAssociativyContext> NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            TAssociativyContext associativyContext,
            IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> connectionManager,
            IMind<TNodeToNodeConnectorRecord, TAssociativyContext> mind,
            INodeManager<TAssociativyContext> nodeManager)
            : base(associativyContext)
        {
            _connectionManager = connectionManager;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}