using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> Mind { get; }
        INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> NodeManager { get; }
    }
}
