﻿using System.Collections.Generic;
using System.Linq;
using Associativy.FrontendEngines.ViewModels;
using Associativy.Models;
using ClaySharp;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using QuickGraph;

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
                    TemplateName: "FrontendEngines/SearchForm",
                    Model: searchViewModel,
                    Prefix: null);
        }

        public virtual dynamic GraphShape(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            return GraphShape<IGraphResultViewModel, IGraphNodeViewModel<TNode>>(graph);
        }

        // Can be used in derived classes to switch the ISearchViewModel implementation
        protected virtual dynamic GraphShape<TGraphResultViewModel, TGraphNodeViewModel>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
            where TGraphResultViewModel : IGraphResultViewModel
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

            // Necessary as shapes and views can't be generic. The nodes can be casted to the
            // appropriate type as necessary.
            var nodes = viewNodes.ToDictionary(item => item.Key, item => item.Value as IGraphNodeViewModel);

            var graphResultViewModel = _workContextAccessor.GetContext().Resolve<TGraphResultViewModel>();
            graphResultViewModel.Nodes = nodes;

            return _shapeFactory.DisplayTemplate(
                TemplateName: "FrontendEngines/Engines/" + Name + "/Graph",
                Model: graphResultViewModel,
                Prefix: null);
        }

        public virtual dynamic GraphResultShape(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            return GraphResultShape(SearchFormShape(), GraphShape(graph));
        }

        public virtual dynamic GraphResultShape(dynamic searchFormShape, dynamic graphShape)
        {
            dynamic New = new ClayFactory();
            var model = New.Model();
            model.SearchForm = searchFormShape;
            model.Result = graphShape;

            return _shapeFactory.DisplayTemplate(
                TemplateName: "FrontendEngines/Result",
                Model: model,
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
            return GraphResultShape(
                    SearchFormShape(searchViewModel),
                    _shapeFactory.DisplayTemplate(
                        TemplateName: "FrontendEngines/NotFound",
                        Model: searchViewModel,
                        Prefix: null)
                );
        }
    }
}