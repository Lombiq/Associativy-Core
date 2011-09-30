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

namespace Associativy.Controllers
{
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : Controller
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        private readonly TAssocociativyServices _associativyServices;
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        public AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices)
        {
            _associativyServices = associativyService;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public ActionResult ShowWholeGraph()
        {
            var sw = new Stopwatch();
            sw.Start();

            var graph = _associativyServices.Mind.GetAllAssociations();

            // Vagy lehetne egyszerűen átadni az egész gráfot? A generics miatt bajos lehet
            //foreach (var edge in graph.Edges)
            //{
            //    edge.Source.Label
            //        edge.Target.Label
            //}

            var viewEdges = new List<GraphEdgeViewModel>();
            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                viewEdges.Add(new GraphEdgeViewModel { SourceLabel = edge.Source.Label, TargetLabel = edge.Target.Label });
            }

            var viewNodes = graph.Vertices.Select(node => node.Label).ToList<string>();

            sw.Stop();
            var x = sw.ElapsedMilliseconds;

            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();
            return new ShapeResult(this, _orchardServices.New.AssociationsGraph(Nodes: viewNodes, Edges: viewEdges));
        }

        //[HttpPost]
        public ActionResult ShowAssociations()
        {
            var searched = new List<TNodePart>();
            searched.Add(_associativyServices.NodeManager.Get("őselem"));
            var graph = _associativyServices.Mind.MakeAssociations(searched);

            var viewEdges = new List<GraphEdgeViewModel>();
            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                viewEdges.Add(new GraphEdgeViewModel { SourceLabel = edge.Source.Label, TargetLabel = edge.Target.Label });
            }

            var viewNodes = graph.Vertices.Select(node => node.Label).ToList<string>();


            _orchardServices.WorkContext.Layout.Title = T("Associations for {0}", "őselem").ToString();
            return new ShapeResult(this, _orchardServices.New.AssociationsGraph(Nodes: viewNodes, Edges: viewEdges));





            var viewModel = new SearchViewModel();
            var z = TryUpdateModel<SearchViewModel>(viewModel);
            if (ModelState.IsValid)
            {
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    //_notifier.Error(T(error));
                }

                return null;
            }

            return new ShapeResult(this, _orchardServices.New.Graph());
        }

        public JsonResult FetchSimilarTerms(string term)
        {
            return Json(_associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }
    }
}
