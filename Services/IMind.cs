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
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GetAllAssociations(IMindSettings settings = null);

        /// <summary>
        /// Makes associations after the specified terms
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeAssociations(IEnumerable<IContent> nodes, IMindSettings settings = null);
    }

    /// <summary>
    /// Service for generating associations
    /// </summary>
    public interface IMind : IMind<IGraphDescriptor>, IDependency
    {
    }
}
