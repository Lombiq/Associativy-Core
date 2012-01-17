using System;
using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    /// <typeparam name="TGraphDescriptor">Type of the IAssociativyGraphDescriptor to use</typeparam>
    public interface IMind<TGraphDescriptor> : IAssociativyService
        where TGraphDescriptor : IGraphDescriptor
    {
        /// <summary>
        /// Returns the whole association graph
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="queryModifier">Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance.</param>
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IMindSettings settings = null, Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="settings"></param>
        /// <param name="queryModifier">Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance.</param> 
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings = null, Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null);
    }

    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind : IMind<IGraphDescriptor>, IDependency
    {
    }
}
