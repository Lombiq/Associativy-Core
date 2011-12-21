using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.DisplayManagement;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;
using QuickGraph;
using Associativy.Models;
using System.Dynamic;
using ClaySharp;
using Orchard.Environment.Extensions;

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
            var searchViewModel = _workContextAccessor.GetContext().Resolve<ISearchViewModel>();

            if (updater != null) updater.TryUpdateModel(searchViewModel, null, null, null);

            return searchViewModel;
        }

        public virtual dynamic SearchFormShape(ISearchViewModel searchViewModel = null)
        {
            if (searchViewModel == null) searchViewModel = GetSearchViewModel();
            

            return _shapeFactory.DisplayTemplate(
                    TemplateName: "FrontendEngines/SearchForm",
                    Model: searchViewModel,
                    Prefix: null);
        }

        public virtual dynamic GraphShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph)
        {
            var viewNodes = new Dictionary<int, IGraphNodeViewModel<TNode>>(graph.VertexCount);

            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                if (!viewNodes.ContainsKey(edge.Source.Id))
                {
                    viewNodes[edge.Source.Id] = _workContextAccessor.GetContext().Resolve<IGraphNodeViewModel<TNode>>();
                    viewNodes[edge.Source.Id].MapFromNode(edge.Source);
                }
                viewNodes[edge.Source.Id].NeighbourIds.Add(edge.Target.Id);

                if (!viewNodes.ContainsKey(edge.Target.Id))
                {
                    viewNodes[edge.Target.Id] = _workContextAccessor.GetContext().Resolve<IGraphNodeViewModel<TNode>>();
                    viewNodes[edge.Target.Id].MapFromNode(edge.Target);
                }
                viewNodes[edge.Target.Id].NeighbourIds.Add(edge.Source.Id);
            }

            // Necessary as shapes and views can't be generic. The nodes can be casted to the
            // appropriate type as necessary.
            var nodes = viewNodes.ToDictionary(item => item.Key, item => item.Value as IGraphNodeViewModel);

            var graphResultViewModel = _workContextAccessor.GetContext().Resolve<IGraphResultViewModel>();
            graphResultViewModel.Nodes = nodes;

            return _shapeFactory.DisplayTemplate(
                TemplateName: "FrontendEngines/Engines/" + Name + "/Graph",
                Model: graphResultViewModel,
                Prefix: null);
        }

        public virtual dynamic GraphResultShape(UndirectedGraph<TNode, UndirectedEdge<TNode>> graph)
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