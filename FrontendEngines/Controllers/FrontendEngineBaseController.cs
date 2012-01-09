﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Associativy.Controllers;
using Associativy.Models;
using Associativy.Models.Mind;
using Associativy.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using QuickGraph;
using Associativy.FrontendEngines.Shapes;
using Associativy.FrontendEngines.NodeFilters;
using Associativy.FrontendEngines.Services;
using System.Diagnostics;

namespace Associativy.FrontendEngines.Controllers
{
    /// <summary>
    /// Base class for frontend engine controllers
    /// </summary>
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class FrontendEngineBaseController : AssociativyBaseController, IUpdateModel
    {
        protected readonly IOrchardServices _orchardServices;
        protected readonly IContentManager _contentManager;
        protected readonly IFrontendShapes _frontendShapes;
        protected readonly dynamic _shapeFactory;
        protected readonly IGraphFilterer _graphFilterer;

        protected virtual string FrontendEngine
        {
            get { return ""; }
        }

        protected virtual Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> GraphQueryModifier
        {
            get { return (query) => query.Join<RoutePartRecord>(); }
        }

        protected string _graphShapeTemplateName;

        public Localizer T { get; set; }

        public FrontendEngineBaseController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory,
            IGraphFilterer graphFilterer)
            : base(associativyServices)
        {
            _orchardServices = orchardServices;
            _contentManager = orchardServices.ContentManager;
            _frontendShapes = frontendShapes;
            _shapeFactory = shapeFactory;
            _graphFilterer = graphFilterer;

            T = NullLocalizer.Instance;
            _graphShapeTemplateName = "FrontendEngines/Engines/" + FrontendEngine + "/Graph";
        }

        public virtual ActionResult ShowWholeGraph()
        {
            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = _associativyServices.Context.MaxZoomLevel;
            
            return new ShapeResult(this, _frontendShapes.SearchResultShape(
                    _frontendShapes.SearchBoxShape(_contentManager.New("AssociativySearchForm")),
                    GraphShape(_mind.GetAllAssociations(settings, GraphQueryModifier)))
                );
        }

        public virtual ActionResult ShowAssociations()
        {
            var searchForm = _contentManager.New("AssociativySearchForm");
            _contentManager.UpdateEditor(searchForm, this);

            if (ModelState.IsValid)
            {
                _orchardServices.WorkContext.Layout.Title = T("Associations for {0}", searchForm.As<SearchFormPart>().Terms).ToString();

                var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
                settings.ZoomLevel = _associativyServices.Context.MaxZoomLevel;

                IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph;
                if (TryGetGraph(searchForm, out graph, settings, GraphQueryModifier))
                {
                    return new ShapeResult(this, _frontendShapes.SearchResultShape(
                            _frontendShapes.SearchBoxShape(searchForm),
                            GraphShape(graph))
                        );
                }
                else
                {
                    return new ShapeResult(this, _frontendShapes.SearchResultShape(
                            _frontendShapes.SearchBoxShape(searchForm),
                            _frontendShapes.AssociationsNotFoundShape(searchForm))
                        );
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
            return Json(_nodeManager.GetSimilarTerms(term), JsonRequestBehavior.AllowGet);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        protected virtual dynamic GraphShape(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph)
        {
            var sw = new Stopwatch();
            sw.Start();
            var viewModels = _graphFilterer.RunFilters(graph, FrontendEngine);
            sw.Stop();

            return _shapeFactory.DisplayTemplate(
                TemplateName: _graphShapeTemplateName,
                Model: graph,
                Prefix: null);
        }

        protected virtual bool TryGetGraph(
            IContent searchForm,
            out IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph,
            IMindSettings settings = null,
             Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> queryModifier = null)
        {
            var searchFormPart = searchForm.As<SearchFormPart>();

            if (searchFormPart.TermsArray.Length == 0)
            {
                graph = null;
                return false;
            }

            var searched = new List<IContent>(searchFormPart.TermsArray.Length);
            foreach (var term in searchFormPart.TermsArray)
            {
                var node = _associativyServices.NodeManager.Get(term);
                if (node == null)
                {
                    graph = null;
                    return false;
                }
                searched.Add(node);
            }
            graph = _mind.MakeAssociations(searched, settings, queryModifier);

            return !graph.IsVerticesEmpty;
        }
    }
}
