﻿using System;
using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    /// <summary>
    /// Service for handling nodes
    /// <typeparam name="TAssociativyContext">Type of the IAssociativyContext to use</typeparam>
    /// </summary>
    public interface INodeManager<TAssociativyContext> : IDependency
        where TAssociativyContext : IAssociativyContext
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

        IContentQuery<ContentItem> GetManyQuery(IEnumerable<int> ids);

        /// <summary>
        /// Gets the node with the specified label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IContent Get(string label);

        /// <summary>
        /// Gets the nodes with the specified ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        //IEnumerable<TNodePart> GetMany(IEnumerable<int> ids);
    }
}
