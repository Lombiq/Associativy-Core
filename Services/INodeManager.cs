using System.Collections.Generic;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// </summary>
    public interface INodeManager
    {
        /// <summary>
        /// Query for customized retrieving of nodes
        /// </summary>
        IContentQuery<ContentItem> GetQuery();

        /// <summary>
        /// Query for retrieving multiple items
        /// </summary>
        /// <param name="ids">Ids of nodes</param>
        IContentQuery<ContentItem> GetManyQuery(IEnumerable<int> ids);

        /// <summary>
        /// Lists nodes that have labels similar to the given snippet
        /// </summary>
        /// <param name="labelSnippet">The snippet of the nodes's label to search for</param>
        IContentQuery<ContentItem> GetSimilarNodesQuery(string labelSnippet);

        /// <summary>
        /// Gets the nodes with the specified labels
        /// </summary>
        /// <param name="labels">The labels</param>
        IContentQuery<ContentItem> GetManyByLabelQuery(IEnumerable<string> labels);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label">The label</param>
        IContentQuery<ContentItem> GetByLabelQuery(string label);

        /// <summary>
        /// Creates a graph of content items from a graph of content item IDs
        /// </summary>
        /// <param name="idGraph">Graph of the IDs</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
    }
}
