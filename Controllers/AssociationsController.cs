using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Associativy.Models;
using Associativy.Services;
using Associativy.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using QuickGraph;
using Associativy.FrontendEngines.ViewModels;
using Associativy.FrontendEngines;

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
            _frontendEngineDriver = frontendEngineDriverLocator.GetDriver("Dracula");

            T = NullLocalizer.Instance;
        }

        public ActionResult ShowWholeGraph()
        {
            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            return new ShapeResult(
                    this,
                    _frontendEngineDriver.GraphResultShape(_associativyServices.Mind.GetAllAssociations(useCache: true))
                );
        }

        public ActionResult ShowAssociations()
        {
            var useSimpleAlgorithm = false;

            var searchFormShape = _frontendEngineDriver.SearchFormShape(updater: this);

            if (ModelState.IsValid)
            {
                var searched = new List<TNodePart>(viewModel.TermsArray.Length);
                foreach (var term in viewModel.TermsArray)
                {
                    var node = _associativyServices.NodeManager.Get(term);
                    if (node == null) return AssociationsNotFound<TSearchViewModel>(viewModel);
                    searched.Add(node);
                }

                var associationsGraph = _associativyServices.Mind.MakeAssociations(searched, useSimpleAlgorithm);

                if (associationsGraph != null)
                {
                    _orchardServices.WorkContext.Layout.Title = T("Associations for {0}", String.Join<string>(", ", viewModel.TermsArray)).ToString();

                    return _frontendEngineDriver.GraphResultShape(
                        searchFormShape,
                        _frontendEngineDriver.GraphShape(associationsGraph));
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

        public JsonResult FetchSimilarTerms(string term)
        {
            return Json(_associativyServices.NodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
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

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
