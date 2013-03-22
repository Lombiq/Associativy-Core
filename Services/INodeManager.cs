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
        IContentQuery<ContentItem> GetBySimilarLabelQuery(string labelSnippet);

        /// <summary>
        /// Gets node(s) with the specified label(s)
        /// </summary>
        /// <param name="labels">Label(s) to search for</param>
        IContentQuery<ContentItem> GetByLabelQuery(params string[] labels);

        /// <summary>
        /// Creates a graph of content items from a graph of content item IDs
        /// </summary>
        /// <param name="idGraph">Graph of the IDs</param>
        IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IUndirectedGraph<int, IUndirectedEdge<int>> idGraph);
    }
}
