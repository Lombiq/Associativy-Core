using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using QuickGraph;
using Associativy.Models.Mind;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    /// <typeparam name="TNodePart">Content part type for nodes</typeparam>
    /// <typeparam name="TNodePartRecord">Content part record type for nodes</typeparam>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public interface IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        /// <summary>
        /// Returns the whole association graph
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> GetAllAssociations(IMindSettings settings = null);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> MakeAssociations(IList<TNodePart> nodes, IMindSettings settings = null);
    }
}
