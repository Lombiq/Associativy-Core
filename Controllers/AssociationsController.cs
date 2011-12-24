using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Associativy.FrontendEngines;
using Associativy.Models;
using Associativy.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using QuickGraph;
using Associativy.FrontendEngines.ViewModels;

namespace Associativy.Controllers
{
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Controller, IUpdateModel
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly TAssocociativyServices _associativyServices;
        protected readonly IOrchardServices _orchardServices;
        protected IFrontendEngineDriver<TNodePart> _frontendEngineDriver;

        public Localizer T { get; set; }

        protected AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices,
            IFrontendEngineDriverLocator<TNodePart> frontendEngineDriverLocator)
        {
            _associativyServices = associativyService;
            _orchardServices = orchardServices;
            _frontendEngineDriver = frontendEngineDriverLocator.GetDriver("JIT");

            T = NullLocalizer.Instance;
        }

        public ActionResult ShowWholeGraph()
        {
            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            return new ShapeResult(
                    this,
                    _frontendEngineDriver.SearchResultShape(_associativyServices.Mind.GetAllAssociations())
                );
        }

        public ActionResult ShowAssociations()
        {
            var searchViewModel = _frontendEngineDriver.GetSearchViewModel(this);

            if (ModelState.IsValid)
            {
                _orchardServices.WorkContext.Layout.Title = T("Associations for {0}", searchViewModel.Terms).ToString();

                IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph;
                if (TryGetGraph(searchViewModel, out graph))
                {
                    return new ShapeResult(
                        this,
                        _frontendEngineDriver.SearchResultShape(
                            _frontendEngineDriver.SearchFormShape(searchViewModel),
                            _frontendEngineDriver.GraphShape(graph))
                        );
                }
                else
                {
                    return new ShapeResult(this, _frontendEngineDriver.AssociationsNotFoundShape(searchViewModel));
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

        public JsonResult FetchSimilarTerms(string term)
        {
            return Json(_associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }

        // No performance loss if with the same params as ShowAssociations because the solution 
        // is cached after ShowAssociations()
        public JsonResult FetchAssociations(int zoomLevel = 0)
        {
            object jsonData = null;
            var searchViewModel = _frontendEngineDriver.GetSearchViewModel(this);
            
            if (!ModelState.IsValid) jsonData = null;
            else
            {
                IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph;

                var graphSettings = _orchardServices.WorkContext.Resolve<IGraphSettings>();
                graphSettings.ZoomLevel = zoomLevel;

                if (TryGetGraph(searchViewModel, out graph, graphSettings: graphSettings))
                {
                    jsonData = _frontendEngineDriver.GraphJson(graph);
                }
                else
                {
                    jsonData = null;
                }
            }

            var json = new JsonResult()
            {
                Data = jsonData,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return json;
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        protected bool TryGetGraph(ISearchViewModel searchViewModel, out IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph, IMindSettings mindSettings = null, IGraphSettings graphSettings = null)
        {
            var searched = new List<TNodePart>(searchViewModel.TermsArray.Length);
            foreach (var term in searchViewModel.TermsArray)
            {
                var node = _associativyServices.NodeManager.Get(term);
                if (node == null)
                {
                    graph = null;
                    return false;
                }
                searched.Add(node);
            }

            graph = _associativyServices.Mind.MakeAssociations(searched, mindSettings, graphSettings);

            return !graph.IsVerticesEmpty;
        }
    }
}
