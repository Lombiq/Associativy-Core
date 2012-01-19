using System;
using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;
using Orchard.ContentManagement;
using QuickGraph;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind : IDependency
    {
        /// <summary>
        /// Returns the whole association graph
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IGraphContext graphContext, IMindSettings settings = null);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IGraphContext graphContext, IEnumerable<IContent> nodes, IMindSettings settings = null);
    }
}
