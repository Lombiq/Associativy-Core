using System;
using QuickGraph;
using Associativy.Models;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines
{
    /// <summary>
    /// Display driver (much like Orchard drivers) for frontend engines
    /// </summary>
    /// <typeparam name="TNode">Node type</typeparam>
    public interface IFrontendEngineDriver<TNode>
        where TNode : INode
    {
        /// <summary>
        /// Returns a new search view model instance
        /// </summary>
        /// <param name="updater">If specified, the view model will also be updated with the updater</param>
        /// <returns>A new search view model instance</returns>
        ISearchViewModel GetSearchViewModel(IUpdateModel updater = null);

        /// <summary>
        /// Returns the shape of the node search form
        /// </summary>
        /// <param name="searchViewModel">If specified, the view model will be used to populate the form</param>
        /// <returns>The shape of the node search form</returns>
        dynamic SearchFormShape(ISearchViewModel searchViewModel = null);

        /// <summary>
        /// Returns the shape of the graph
        /// </summary>
        /// <param name="graph">The graph to display</param>
        /// <returns>The shape of the graph</returns>
        dynamic GraphShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph);

        /// <summary>
        /// Returns the result shape containing the search form and the graph
        /// </summary>
        /// <param name="graph">The graph to display</param>
        /// <returns>The result shape containing the search from and the graph</returns>
        dynamic GraphResultShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph);

        /// <summary>
        /// Returns the result shape containing the search form and the graph
        /// </summary>
        /// <param name="searchFormShape">Shape of the search form</param>
        /// <param name="graphShape">Shape of the graph</param>
        /// <returns>The result shape containing the search from and the graph</returns>
        dynamic GraphResultShape(dynamic searchFormShape, dynamic graphShape);

        /// <summary>
        /// Returns the not found shape
        /// </summary>
        /// <param name="searchViewModel">The search view model instancej of the search</param>
        /// <returns>The not found shape</returns>
        dynamic AssociationsNotFoundShape(ISearchViewModel searchViewModel);
    }
}
