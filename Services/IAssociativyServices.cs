using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    public interface IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> ConnectionManager { get; }
        IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> Mind { get; }
        INodeManager<TNodePart, TNodePartRecord> NodeManager { get; }
    }
}
