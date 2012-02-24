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
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="settings">Mind settings</param>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IGraphContext graphContext, IMindSettings settings = null);

        /// <summary>
        /// Makes associations upon the specified nodes
        /// </summary>
        /// <param name="graphContext">The IGraphContext instance to use with the operation</param>
        /// <param name="nodes">The nodes to search associations between</param>
        /// <param name="settings">Mind settings</param>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IGraphContext graphContext, IEnumerable<IContent> nodes, IMindSettings settings = null);
    }
}
