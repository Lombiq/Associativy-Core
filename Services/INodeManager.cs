using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Associativy.GraphDiscovery;

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
        IContentQuery<ContentItem> GetContentQuery(IGraphContext graphContext);

        /// <summary>
        /// Query for retrieving multiple items
        /// </summary>
        /// <param name="ids">Ids of nodes</param>
        /// <returns></returns>
        IContentQuery<ContentItem> GetManyContentQuery(IGraphContext graphContext, IEnumerable<int> ids);

        /// <summary>
        /// Lists nodes that have labels similar to the given snippet
        /// </summary>
        /// <param name="labelSnippet">The snippet of the nodes's label to search for</param>
        /// <param name="maxCount">Maximal number of items returned</param>
        /// <returns>Terms similar to the snippet</returns>
        IEnumerable<IContent> GetSimilarNodes(IGraphContext graphContext, string labelSnippet, int maxCount = 10, QueryHints queryHints = null);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IContent Get(IGraphContext graphContext, string label, QueryHints queryHints = null);

        IEnumerable<IContent> GetMany(IGraphContext graphContext, IEnumerable<string> labels, QueryHints queryHints = null);
    }
}
