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

            // Vagy lehetne egyszerűen átadni az egész gráfot? A generics miatt bajos lehet
            //foreach (var edge in graph.Edges)
            //{
            //    edge.Source.Label
            //        edge.Target.Label
            //}

            sw.Stop();
            var x = sw.ElapsedMilliseconds;

            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            return GraphShape(_associativyServices.Mind.GetAllAssociations());
        }

        //[HttpPost]
        public ActionResult ShowAssociations()
        {
            var useSimpleAlgorithm = false;

            var viewModel = new SearchViewModel();
            TryUpdateModel<SearchViewModel>(viewModel);

            //if (ModelState.IsValid)
            if (true)
            {
                var searched = new List<TNodePart>();
                foreach (var term in viewModel.TermsArray)
                {
                    var node = _associativyServices.NodeManager.Get(term);
                    if (node == null) return AssociationsNotFound();
                    searched.Add(node);
                }

                searched.Add(_associativyServices.NodeManager.Get("tűz")); // 26
                searched.Add(_associativyServices.NodeManager.Get("víz")); // 22
                //searched.Add(_associativyServices.NodeManager.Get("autó")); // 36

                var associationsGraph = _associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm);

                if (associationsGraph != null)
                {
                    var terms = String.Join<string>(", ", viewModel.TermsArray);
                    _orchardServices.WorkContext.Layout.Title = T("Associations for {0}", terms).ToString();

                    return GraphShape(_associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm));
                }
                else
                {
                    return AssociationsNotFound();
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

        protected ActionResult AssociationsNotFound()
        {
            return null;
        }

        public JsonResult FetchSimilarTerms(string term)
        {
            return Json(_associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }

        protected ShapeResult GraphShape(UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> graph)
        {
            var viewEdges = new List<GraphEdgeViewModel>();
            var edges = graph.Edges.ToList();
            foreach (var edge in edges)
            {
                viewEdges.Add(new GraphEdgeViewModel { SourceLabel = edge.Source.Label, TargetLabel = edge.Target.Label });
            }

            var viewNodes = graph.Vertices.Select(node => node.Label).ToList<string>();

            return new ShapeResult(this, _orchardServices.New.AssociationsGraph(Nodes: viewNodes, Edges: viewEdges));
        }
    }
}
