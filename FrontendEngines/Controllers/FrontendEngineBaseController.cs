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
using Associativy.Models.Mind;
using Associativy.Controllers;

namespace Associativy.FrontendEngines.Controllers
{
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class FrontendEngineBaseController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : AssociativyBaseController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, IUpdateModel
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IOrchardServices _orchardServices;
        protected IFrontendEngineDriver<TNodePart> _frontendEngineDriver;

        protected virtual string FrontendEngineDriver
        {
            get { return ""; }
        }

        public Localizer T { get; set; }

        protected FrontendEngineBaseController(
            TAssocociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendEngineDriver<TNodePart> frontendEngineDriver)
            : base(associativyServices)
        {
            _orchardServices = orchardServices;
            _frontendEngineDriver = frontendEngineDriver;

            T = NullLocalizer.Instance;
        }

        public virtual ActionResult ShowWholeGraph()
        {
            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = 5;

            return new ShapeResult(
                    this,
                    _frontendEngineDriver.SearchResultShape(_mind.GetAllAssociations(settings))
                );
        }

        public virtual ActionResult ShowAssociations()
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

        public virtual JsonResult FetchSimilarTerms(string term)
        {
            return Json(_associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        protected virtual bool TryGetGraph(ISearchViewModel searchViewModel, out IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph, IMindSettings settings = null)
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
            graph = _mind.MakeAssociations(searched, settings);

            return !graph.IsVerticesEmpty;
        }
    }
}
