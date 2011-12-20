using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using QuickGraph;

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
        /// <param name="zoomLevel"></param>
        /// <param name="useCache"></param>
        UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociations(int zoomLevel = 0, bool useCache = true);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="simpleAlgorithm"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="useCache"></param>
        UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<TNodePart> terms, bool simpleAlgorithm = false, int zoomLevel = 0, bool useCache = true);
    }
}
