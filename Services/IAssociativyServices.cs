using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        IConnectionManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> ConnectionManager { get; }
        IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> Mind { get; }
        INodeManager<TNodePart, TNodePartRecord, TNodeParams> NodeManager { get; }
    }
}
