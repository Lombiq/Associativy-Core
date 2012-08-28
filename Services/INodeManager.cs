using System.Collections.Generic;
using Associativy.GraphDiscovery;
using Orchard;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// </summary>
    public interface INodeManager : IDependency
    {
        /// <summary>
        /// Query for customized retrieving of nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        IContentQuery<ContentItem> GetContentQuery(IGraphContext graphContext);

        /// <summary>
        /// Query for retrieving multiple items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="ids">Ids of nodes</param>
        IContentQuery<ContentItem> GetManyContentQuery(IGraphContext graphContext, IEnumerable<int> ids);

        /// <summary>
        /// Lists nodes that have labels similar to the given snippet
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="labelSnippet">The snippet of the nodes's label to search for</param>
        /// <param name="maxCount">Maximal number of items returned</param>
        /// <param name="queryHints">The query hints to use with the content query</param>
        /// <returns>Terms similar to the snippet</returns>
        IEnumerable<IContent> GetSimilarNodes(IGraphContext graphContext, string labelSnippet, int maxCount = 10, QueryHints queryHints = null);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="label">The label</param>
        /// <param name="queryHints">The query hints to use with the content query</param>
        IContent Get(IGraphContext graphContext, string label, QueryHints queryHints = null);

        /// <summary>
        /// Gets the nodes with the specified labels
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="labels">The labels</param>
        /// <param name="queryHints">The query hints to use with the content query</param>
        IEnumerable<IContent> GetMany(IGraphContext graphContext, IEnumerable<string> labels, QueryHints queryHints = null);
    }
}
