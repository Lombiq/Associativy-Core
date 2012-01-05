using System;
using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard.ContentManagement;
using QuickGraph;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind : IAssociativyService, IDependency
    {
        /// <summary>
        /// Returns the whole association graph
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="queryModifier">Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance.</param>
        /// <returns></returns>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IMindSettings settings = null, Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="settings"></param>
        /// <param name="queryModifier">Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance.</param> 
        /// <returns></returns>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings = null, Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null);
    }

    /// <summary>
    /// Service for generating associations
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    /// <typeparam name="TAssociativyContext">Type of the IAssociativyContext to use</typeparam>
    public interface IMind<TNodeToNodeConnectorRecord, TAssociativyContext> : IMind
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
    }
}
