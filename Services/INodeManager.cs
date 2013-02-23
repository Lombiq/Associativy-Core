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
        IContentQuery<ContentItem> GetQuery(IGraphContext graphContext);

        /// <summary>
        /// Query for retrieving multiple items
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="ids">Ids of nodes</param>
        IContentQuery<ContentItem> GetManyQuery(IGraphContext graphContext, IEnumerable<int> ids);

        /// <summary>
        /// Lists nodes that have labels similar to the given snippet
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="labelSnippet">The snippet of the nodes's label to search for</param>
        IContentQuery<ContentItem> GetSimilarNodesQuery(IGraphContext graphContext, string labelSnippet);

        /// <summary>
        /// Gets the nodes with the specified labels
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="labels">The labels</param>
        IContentQuery<ContentItem> GetManyByLabelQuery(IGraphContext graphContext, IEnumerable<string> labels);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="label">The label</param>
        IContentQuery<ContentItem> GetByLabelQuery(IGraphContext graphContext, string label);
    }
}
