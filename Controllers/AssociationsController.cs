using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;
using Orchard;
using Orchard.Mvc;
using Associativy.Services;
using Associativy.Models;
using Associativy.ViewModels;
using System.Diagnostics;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using QuickGraph;

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

        public Localizer T { get; set; }

        public AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices)
        {
            this.associativyServices = associativyService;
            this.orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        protected ActionResult ShowWholeGraph<TGraphNodeViewModel>()
            where TGraphNodeViewModel : GraphNodeViewModel<TNodePart>, new()
        {
            var sw = new Stopwatch();
            sw.Start();

            sw.Stop();
            var x = sw.ElapsedMilliseconds;

            orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            return GraphResultShape(
                        SearchFormShape(
                            new SearchViewModel()
                        ),
                        GraphShape<TGraphNodeViewModel>(
                            associativyServices.Mind.GetAllAssociations()
                        )
                        );

            //return GraphShape<TGraphNodeViewModel>(associativyServices.Mind.GetAllAssociations());
        }

        protected ActionResult ShowAssociations<TGraphNodeViewModel>()
            where TGraphNodeViewModel : GraphNodeViewModel<TNodePart>, new()
        {
            var useSimpleAlgorithm = false;

            var viewModel = new SearchViewModel();
            TryUpdateModel<SearchViewModel>(viewModel);

            if (ModelState.IsValid)
            //if (true)
            {
                var searched = new List<TNodePart>(viewModel.TermsArray.Length);
                foreach (var term in viewModel.TermsArray)
                {
                    var node = associativyServices.NodeManager.Get(term);
                    if (node == null) return AssociationsNotFound(viewModel);
                    searched.Add(node);
                }

                //searched.Add(_associativyServices.NodeManager.Get("tűz")); // 26
                //searched.Add(_associativyServices.NodeManager.Get("víz")); // 22
                //searched.Add(_associativyServices.NodeManager.Get("levegő")); // 30
                //searched.Add(_associativyServices.NodeManager.Get("autó")); // 36

                var associationsGraph = associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm);

                if (associationsGraph != null)
                {
                    orchardServices.WorkContext.Layout.Title = T("Associations for {0}", String.Join<string>(", ", viewModel.TermsArray)).ToString();

                    return GraphResultShape(
                        SearchFormShape(viewModel), 
                        GraphShape<TGraphNodeViewModel>(associationsGraph)
                        );
                    //return GraphShape<TGraphNodeViewModel>(associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm));
                }
                else
                {
                    return AssociationsNotFound(viewModel);
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

        protected ActionResult AssociationsNotFound(SearchViewModel viewModel)
        {
            return new ShapeResult(this, orchardServices.New.Graphs_NotFound(ViewModel: viewModel));
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

        protected ShapeResult GraphResultShape(ShapeResult searchFormShape, ShapeResult graphShape)
        {
            return new ShapeResult(this, 
                orchardServices.New.Graphs_Result(
                SearchForm: searchFormShape,
                Graph: graphShape)
                );
        }

        protected ShapeResult GraphShape<TGraphNodeViewModel>(UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> graph)
            where TGraphNodeViewModel : GraphNodeViewModel<TNodePart>, new()
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
            return new ShapeResult(this, orchardServices.New.Graphs_Dracula(Nodes: nodes));

            //var searchFormPart = orchardServices.ContentManager.New<SearchFormPart>("NotionSearchFormWidget");
            //var searchForm = orchardServices.ContentManager.BuildDisplay(
            //    searchFormPart
            //    );

            //return new ShapeResult(this, orchardServices.New.Graphs_Default(SearchForm: searchForm, Nodes: nodes));
        }

        protected ShapeResult SearchFormShape(SearchViewModel searchViewModel)
        {
            var searchFormPart = orchardServices.ContentManager.New<SearchFormPart>("NotionSearchFormWidget");
            searchFormPart.Terms = searchViewModel.Terms;

            return new ShapeResult(this, 
                orchardServices.ContentManager.BuildDisplay(
                    searchFormPart
                    )
                );
        }
    }
}
