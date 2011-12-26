using System;
using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// </summary>
    /// <typeparam name="TNodePart">Content part type for nodes</typeparam>
    /// <typeparam name="TNodePartRecord">Content part record type for nodes</typeparam>
    public interface INodeManager<TNodePart, TNodePartRecord> : IDependency
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
    {
        /// <summary>
        /// Lists terms similar to the snippet
        /// </summary>
        /// <param name="snippet">The snippet to search for</param>
        /// <param name="maxCount">Maximal number of items returned</param>
        /// <returns>Terms similar to the snippet</returns>
        IList<string> GetSimilarTerms(string snippet, int maxCount = 10);

        
        #region Node CRUD
        /// <summary>
        /// Query for customized retrieving of nodes
        /// </summary>
        IContentQuery<TNodePart, TNodePartRecord> ContentQuery { get; }

        /// <summary>
        /// Creates a new content item that has TNodePart attached to it with the specified params
        /// </summary>
        /// <param name="nodeParams">The INodeParams object filled with the node's params</param>
        TNodePart Create(INodeParams<TNodePart> nodeParams);

        /// <summary>
        /// Creates a new content item that has TNodePart attached to it
        /// </summary>
        /// <param name="contentType">A suitable content type's name, that has TNodePart attached</param>
        TNodePart New(string contentType);

        /// <summary>
        /// Creates a new node content item
        /// </summary>
        /// <param name="node">The content item</param>
        void Create(ContentItem node);

        /// <summary>
        /// Gets the node with the specified id
        /// </summary>
        TNodePart Get(int id);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        TNodePart Get(string label);

        /// <summary>
        /// Gets the nodes with the specified ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<TNodePart> GetMany(IList<int> ids);
        TNodePart Update(INodeParams<TNodePart> nodeParams);

        /// <summary>
        /// Updates a node content item
        /// </summary>
        /// <param name="node">The TNodePart attached to the content item</param>
        TNodePart Update(TNodePart node);

        /// <summary>
        /// Soft deletes the node and leaves connections intact (that means the whole partial graph can be reconstructed)
        /// </summary>
        /// <param name="id">Id of the node</param>
        void Remove(int id);
        #endregion
    }
}
