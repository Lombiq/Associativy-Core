using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    /// <summary>
    /// Service collector for Associativy services
    /// </summary>
    /// <typeparam name="TNodePart">Content part type for nodes</typeparam>
    /// <typeparam name="TNodePartRecord">Content part record type for nodes</typeparam>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
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
