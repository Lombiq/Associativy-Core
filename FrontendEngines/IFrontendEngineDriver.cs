using System;
using QuickGraph;
using Associativy.Models;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines
{
    public interface IFrontendEngineDriver<TNode>
        where TNode : INode
    {
        ISearchViewModel GetSearchViewModel(IUpdateModel updater = null);
        dynamic SearchFormShape(ISearchViewModel searchViewModel = null);
        dynamic GraphShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph);
        dynamic GraphResultShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph);
        dynamic GraphResultShape(dynamic searchFormShape, dynamic graphShape);
        dynamic AssociationsNotFound(ISearchViewModel searchViewModel);
    }
}
