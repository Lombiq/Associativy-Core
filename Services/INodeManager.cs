using System.Collections.Generic;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// </summary>
    public interface INodeManager : IAssociativyService, IDependency
    {
        /// <summary>
        /// Lists terms similar to the snippet
        /// </summary>
        /// <param name="snippet">The snippet to search for</param>
        /// <param name="maxCount">Maximal number of items returned</param>
        /// <returns>Terms similar to the snippet</returns>
        IEnumerable<string> GetSimilarTerms(string snippet, int maxCount = 10);

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
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IContent Get(string label);
    }

    /// <summary>
    /// Service for handling nodes
    /// </summary>
    /// <typeparam name="TAssociativyContext">Type of the IAssociativyContext to use</typeparam>
    public interface INodeManager<TAssociativyContext> : INodeManager
        where TAssociativyContext : IAssociativyContext
    {
    }
}
