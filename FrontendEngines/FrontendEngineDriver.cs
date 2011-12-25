using System.Collections.Generic;
using System.Linq;
using Associativy.FrontendEngines.ViewModels;
using Associativy.Models;
using ClaySharp;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using QuickGraph;
using System;

namespace Associativy.FrontendEngines
{
    [OrchardFeature("Associativy")]
    public abstract class FrontendEngineDriver<TNode> : IFrontendEngineDriver<TNode>
        where TNode : INode
    {
        protected readonly IOrchardServices _orchardServices;
        protected dynamic _shapeFactory;
        protected readonly IWorkContextAccessor _workContextAccessor;

        protected virtual string Name
        {
            get { return ""; }
        }

        protected virtual string SearchFormShapeTemplateName
        {
            get { return "FrontendEngines/SearchForm"; }

        }

        protected virtual string SearchResultShapeTemplateName
        {
            get { return "FrontendEngines/SearchResult"; }
        }

        protected virtual string GraphShapeTemplateName
        {
            get { return "FrontendEngines/Engines/" + Name + "/Graph"; }
        }

        protected virtual string AssociationsNotFoundShapeTemplateName
        {
            get { return "FrontendEngines/NotFound"; }
        }

        public FrontendEngineDriver(
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
        {
            _orchardServices = orchardServices;
            _shapeFactory = shapeFactory;
            _workContextAccessor = workContextAccessor;
        }

        public virtual ISearchViewModel GetSearchViewModel(IUpdateModel updater = null)
        {
            return GetSearchViewModel<ISearchViewModel>(updater);
        }

        // Can be used in derived classes to switch the ISearchViewModel implementation
        protected virtual TSearchViewModel GetSearchViewModel<TSearchViewModel>(IUpdateModel updater = null)
            where TSearchViewModel : class, ISearchViewModel
        {
            var searchViewModel = _workContextAccessor.GetContext().Resolve<TSearchViewModel>();

            if (updater != null) updater.TryUpdateModel(searchViewModel, null, null, null);

            return searchViewModel;
        }

        public virtual dynamic SearchFormShape(ISearchViewModel searchViewModel = null)
        {
            return SearchFormShape<ISearchViewModel>(searchViewModel);
        }

        // Can be used in derived classes to switch the ISearchViewModel implementation
        protected virtual dynamic SearchFormShape<TSearchViewModel>(TSearchViewModel searchViewModel = null)
            where TSearchViewModel : class, ISearchViewModel
        {
            if (searchViewModel == null) searchViewModel = GetSearchViewModel<TSearchViewModel>();

            return _shapeFactory.DisplayTemplate(
                    TemplateName: SearchFormShapeTemplateName,
                    Model: searchViewModel,
                    Prefix: null);
        }

        public virtual dynamic GraphShape(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            return GraphShape<IGraphViewModel, IGraphNodeViewModel<TNode>>(graph);
        }

        // Can be used in derived classes to switch the IGraphViewModel and IGraphNodeViewModel<> implementation
        protected virtual dynamic GraphShape<TGraphViewModel, TGraphNodeViewModel>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
            where TGraphViewModel : IGraphViewModel
            where TGraphNodeViewModel : IGraphNodeViewModel<TNode>
        {
            // Necessary as shapes and views can't be generic. The nodes can be casted to the
            // appropriate type as necessary.
            var nodes = BuildViewNodes<TGraphNodeViewModel>(graph).ToDictionary(item => item.Key, item => item.Value as IGraphNodeViewModel);

            var graphViewModel = _workContextAccessor.GetContext().Resolve<TGraphViewModel>();
            graphViewModel.Nodes = nodes;

            return _shapeFactory.DisplayTemplate(
                TemplateName: GraphShapeTemplateName,
                Model: graphViewModel,
                Prefix: null);
        }

        protected virtual Dictionary<int, TGraphNodeViewModel> BuildViewNodes<TGraphNodeViewModel>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
            where TGraphNodeViewModel : IGraphNodeViewModel<TNode>
        {
            var viewNodes = new Dictionary<int, TGraphNodeViewModel>(graph.VertexCount);

            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                if (!viewNodes.ContainsKey(edge.Source.Id))
                {
                    viewNodes[edge.Source.Id] = _workContextAccessor.GetContext().Resolve<TGraphNodeViewModel>();
                    viewNodes[edge.Source.Id].MapFromNode(edge.Source);
                }
                viewNodes[edge.Source.Id].Neighbours.Add(edge.Target);

                if (!viewNodes.ContainsKey(edge.Target.Id))
                {
                    viewNodes[edge.Target.Id] = _workContextAccessor.GetContext().Resolve<TGraphNodeViewModel>();
                    viewNodes[edge.Target.Id].MapFromNode(edge.Target);
                }
                viewNodes[edge.Target.Id].Neighbours.Add(edge.Source);
            }

            return viewNodes;
        }

        public virtual dynamic SearchResultShape(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            return SearchResultShape(SearchFormShape(), GraphShape(graph));
        }

        public virtual dynamic SearchResultShape(dynamic searchFormShape, dynamic graphShape)
        {
            return SearchResultShape<ISearchResultViewModel>(searchFormShape, graphShape);
        }

        // Can be used in derived classes to switch the ISearchResultViewModel implementation
        protected virtual dynamic SearchResultShape<TSearchViewModel>(dynamic searchFormShape, dynamic graphShape)
            where TSearchViewModel : ISearchResultViewModel
        {
            var graphResultViewModel = _workContextAccessor.GetContext().Resolve<ISearchResultViewModel>();
            graphResultViewModel.SearchForm = searchFormShape;
            graphResultViewModel.Graph = graphShape;

            return _shapeFactory.DisplayTemplate(
                TemplateName: SearchResultShapeTemplateName,
                Model: graphResultViewModel,
                Prefix: null);
        }

        public virtual dynamic AssociationsNotFoundShape(ISearchViewModel searchViewModel)
        {
            return AssociationsNotFoundShape<ISearchViewModel>(searchViewModel);
        }

        // Can be used in derived classes to switch the ISearchViewModel implementation
        protected virtual dynamic AssociationsNotFoundShape<TSearchViewModel>(TSearchViewModel searchViewModel)
            where TSearchViewModel : class, ISearchViewModel
        {
            return SearchResultShape(
                    SearchFormShape(searchViewModel),
                    _shapeFactory.DisplayTemplate(
                        TemplateName: AssociationsNotFoundShapeTemplateName,
                        Model: searchViewModel,
                        Prefix: null)
                );
        }
    }
}