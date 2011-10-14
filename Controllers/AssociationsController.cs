using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Themes;
using Orchard;
using Orchard.Mvc;
using Associativy.Services;
using Associativy.Models;
using Associativy.ViewModels;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using QuickGraph;
using Orchard.DisplayManagement;

namespace Associativy.Controllers
{
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Controller
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly TAssocociativyServices associativyServices;
        protected readonly IOrchardServices orchardServices;
        protected dynamic shapeFactory;

        public Localizer T { get; set; }

        protected AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory)
        {
            this.associativyServices = associativyService;
            this.orchardServices = orchardServices;
            this.shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;
        }

        protected ActionResult ShowWholeGraph<TSearchViewModel, TGraphResultViewModel, TGraphNodeViewModel>()
            where TSearchViewModel : class, ISearchViewModel, new()
            where TGraphResultViewModel : IGraphResultViewModel, new()
            where TGraphNodeViewModel : IGraphNodeViewModel<TNodePart>, new()
        {
            orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            return GraphResult(
                    SearchFormShape(
                        new TSearchViewModel()
                    ),
                    GraphShape<TGraphResultViewModel, TGraphNodeViewModel>(
                        associativyServices.Mind.GetAllAssociations()
                    )
                );
        }

        protected ActionResult ShowAssociations<TSearchViewModel, TGraphResultViewModel, TGraphNodeViewModel>()
            where TSearchViewModel : class, ISearchViewModel, new()
            where TGraphResultViewModel : IGraphResultViewModel, new()
            where TGraphNodeViewModel : IGraphNodeViewModel<TNodePart>, new()
        {
            var useSimpleAlgorithm = false;

            var viewModel = new TSearchViewModel();
            TryUpdateModel(viewModel);

            if (ModelState.IsValid)
            //if (true)
            {
                var searched = new List<TNodePart>(viewModel.TermsArray.Length);
                foreach (var term in viewModel.TermsArray)
                {
                    var node = associativyServices.NodeManager.Get(term);
                    if (node == null) return AssociationsNotFound<TSearchViewModel>(viewModel);
                    searched.Add(node);
                }

                var associationsGraph = associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm);

                if (associationsGraph != null)
                {
                    orchardServices.WorkContext.Layout.Title = T("Associations for {0}", String.Join<string>(", ", viewModel.TermsArray)).ToString();

                    return GraphResult(
                        SearchFormShape<TSearchViewModel>(viewModel),
                        GraphShape<TGraphResultViewModel, TGraphNodeViewModel>(associationsGraph)
                        );
                }
                else
                {
                    return AssociationsNotFound<TSearchViewModel>(viewModel);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    //_notifier.Error(T(error));
                }

                return null;
            }
        }

        protected ActionResult AssociationsNotFound<TSearchViewModel>(TSearchViewModel viewModel)
            where TSearchViewModel : class, ISearchViewModel, new()
        {
            return GraphResult(
                    SearchFormShape<TSearchViewModel>(viewModel),
                    shapeFactory.DisplayTemplate(
                        TemplateName: "Graphs/NotFound", 
                        Model: viewModel,
                        Prefix: null)
                );
        }

        public JsonResult FetchSimilarTerms(string term)
        {
            return Json(associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }

        // No performance loss if with the same params as ShowAssociations because the solution 
        // is cached after ShowAssociations()
        public JsonResult FetchAssociations()
        {
            //var z = new List<GraphNodeViewModel>();
            //z.Add(new GraphNodeViewModel() { Id = 2, Label = "kkk", NeighbourIds = new List<int>() { 9, 5 } });
            //z.Add(new GraphNodeViewModel() { Id = 5, Label = "sdafsdfdsf", NeighbourIds = new List<int>() { 9, 2 } });
            //return Json(z, JsonRequestBehavior.AllowGet);
            return null;
        }

        protected ShapeResult GraphResult(dynamic searchFormShape, dynamic resultShape)
        {
            return new ShapeResult(this,
                orchardServices.New.Graphs_Result(
                    SearchForm: searchFormShape,
                    Result: resultShape
                    )
                );
        }

        protected dynamic GraphShape<TGraphResultViewModel, TGraphNodeViewModel>(UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> graph)
            where TGraphResultViewModel : IGraphResultViewModel, new()
            where TGraphNodeViewModel : IGraphNodeViewModel<TNodePart>, new()
        {
            var viewNodes = new Dictionary<int, TGraphNodeViewModel>(graph.VertexCount);

            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                if (!viewNodes.ContainsKey(edge.Source.Id))
                {
                    viewNodes[edge.Source.Id] = new TGraphNodeViewModel();
                    viewNodes[edge.Source.Id].MapFromNode(edge.Source);
                }
                viewNodes[edge.Source.Id].NeighbourIds.Add(edge.Target.Id);

                if (!viewNodes.ContainsKey(edge.Target.Id))
                {
                    viewNodes[edge.Target.Id] = new TGraphNodeViewModel();
                    viewNodes[edge.Target.Id].MapFromNode(edge.Target);
                }
                viewNodes[edge.Target.Id].NeighbourIds.Add(edge.Source.Id);
            }

            // Necessary as shapes and views can't be generic. The nodes can be casted to the
            // appropriate type as necessary.
            var nodes = viewNodes.ToDictionary(item => item.Key, item => item.Value as IGraphNodeViewModel);

            // !!!!!!!!!! orchardServices.New["dkdk-2"] = "jjJ";
            // plugin gráfmegjelenítőknek? Delegate?
            return shapeFactory.DisplayTemplate(
                TemplateName: "Graphs/DisplayEngines/Dracula", 
                Model: new TGraphResultViewModel() { Nodes = nodes }, 
                Prefix: null);
        }

        protected dynamic SearchFormShape<TSearchViewModel>(TSearchViewModel searchViewModel)
            where TSearchViewModel : class, ISearchViewModel, new()
        {
            var searchFormPart = orchardServices.ContentManager.New<SearchFormPart>("NotionSearchFormWidget");
            searchFormPart.Terms = searchViewModel.Terms;

            return orchardServices.ContentManager.BuildDisplay(
                    searchFormPart
                    );
        }
    }
}
