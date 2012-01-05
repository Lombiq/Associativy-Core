using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Associativy.Controllers;
using Associativy.Models;
using Associativy.Models.Mind;
using Associativy.Services;
using Associativy.Shapes;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using QuickGraph;

namespace Associativy.FrontendEngines.Controllers
{
    /// <summary>
    /// Base class for frontend engine controllers
    /// </summary>
    [Themed]
    [OrchardFeature("Associativy")]
    public abstract class FrontendEngineBaseController : AssociativyBaseController, IFrontendEngineController, IUpdateModel
    {
        protected readonly IOrchardServices _orchardServices;
        protected readonly IContentManager _contentManager;
        protected readonly IFrontendShapes _shapes;
        protected readonly dynamic _shapeFactory;

        protected virtual string FrontendEngine
        {
            get { return ""; }
        }

        protected virtual Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> GraphQueryModifier
        {
            get { return (query) => query.Join<RoutePartRecord>(); }
        }

        protected virtual string GraphShapeTemplateName
        {
            get { return "FrontendEngines/Engines/" + FrontendEngine + "/Graph"; }
        }

        public Localizer T { get; set; }

        public FrontendEngineBaseController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes shapes,
            IShapeFactory shapeFactory)
            : base(associativyServices)
        {
            _orchardServices = orchardServices;
            _contentManager = orchardServices.ContentManager;
            _shapes = shapes;
            _shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;
        }

        public virtual ActionResult ShowWholeGraph()
        {
            _orchardServices.WorkContext.Layout.Title = T("The whole graph").ToString();

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = 10;

            var sw = new Stopwatch();
            sw.Start();
            _mind.GetAllAssociations(settings, GraphQueryModifier);
            sw.Stop();

            
            return new ShapeResult(this, _shapes.SearchResultShape(
                    _shapes.SearchBoxShape(_contentManager.New("AssociativySearchForm")),
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
                settings.ZoomLevel = 10;

                IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph;
                var sw = new Stopwatch();
                sw.Start();
                TryGetGraph(searchForm, out graph, settings, GraphQueryModifier);
                sw.Stop();
                if (TryGetGraph(searchForm, out graph, settings, GraphQueryModifier))
                {
                    return new ShapeResult(this, _shapes.SearchResultShape(
                            _shapes.SearchBoxShape(searchForm),
                            GraphShape(graph))
                        );
                }
                else
                {
                    return new ShapeResult(this, _shapes.SearchResultShape(
                            _shapes.SearchBoxShape(searchForm),
                            _shapes.AssociationsNotFoundShape(searchForm))
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
            return _shapeFactory.DisplayTemplate(
                TemplateName: GraphShapeTemplateName,
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
