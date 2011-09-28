﻿using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> Mind { get; }
        INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> NodeManager { get; }
    }
}
