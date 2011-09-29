using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;
using Associativy.ViewModels.Notions;
using Orchard;
using Orchard.Mvc;
using Associativy.Services;
using Associativy.Models;
using Associativy.ViewModels;
using System.Diagnostics;
using Orchard.Localization;
using Associativy.Services.Notions;

namespace Associativy.Controllers
{
    [Themed]
    public class AssociationsController : Controller
    {
        private readonly IAssociativyNotionsServices _associativyService;
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        public AssociationsController(
            IAssociativyNotionsServices associativyService,
            IOrchardServices orchardServices)
        {
            _associativyService = associativyService;
            _orchardServices = orchardServices;
            
            T = NullLocalizer.Instance;
        }

        public ActionResult ShowWholeGraph()
        {
            var sw = new Stopwatch();
            sw.Start();

            var graph = _associativyService.Mind.GetAllAssociations();

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
            return new ShapeResult(this, _orchardServices.New.Graph(Nodes: viewNodes, Edges: viewEdges)); 
        }

        public ActionResult ShowAssociations()
        {
            var viewModel = new NotionSearchViewModel();
            var z = TryUpdateModel<NotionSearchViewModel>(viewModel);
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
            return Json(_associativyService.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }
    }
}
