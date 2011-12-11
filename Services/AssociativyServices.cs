﻿using Associativy.Models;
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
        protected readonly IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _connectionManager;
        public IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> ConnectionManager
        {
            get { return _connectionManager; }
        }

        protected readonly IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _mind;
        public IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;
        public INodeManager<TNodePart, TNodePartRecord> NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _connectionManager,
            IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> mind,
            INodeManager<TNodePart, TNodePartRecord> nodeManager)
        {
            _connectionManager = _connectionManager;
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}