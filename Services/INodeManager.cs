using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// </summary>
    public interface INodeManager : IAssociativyService, IDependency
    {
        /// <summary>
        /// Query for customized retrieving of nodes
        /// </summary>
        IContentQuery<ContentItem> ContentQuery { get; }

        /// <summary>
        /// Query for retrieving multiple items
        /// </summary>
        /// <param name="ids">Ids of nodes</param>
        /// <returns></returns>
        IContentQuery<ContentItem> GetManyQuery(IEnumerable<int> ids);

        /// <summary>
        /// Lists nodes that have labels similar to the given snippet
        /// </summary>
        /// <param name="labelSnippet">The snippet of the nodes's label to search for</param>
        /// <param name="maxCount">Maximal number of items returned</param>
        /// <returns>Terms similar to the snippet</returns>
        IEnumerable<IContent> GetSimilarNodes(string labelSnippet, int maxCount = 10, QueryHints queryHints = null);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IContent Get(string label, QueryHints queryHints = null);

        IEnumerable<IContent> GetMany(IEnumerable<string> labels, QueryHints queryHints = null);
    }

    /// <summary>
    /// Service for handling nodes
    /// </summary>
    /// <typeparam name="TAssociativyGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface INodeManager<TAssociativyGraphDescriptor> : INodeManager
        where TAssociativyGraphDescriptor : IAssociativyGraphDescriptor
    {
    }
}
