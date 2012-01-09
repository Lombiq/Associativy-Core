using System.Collections.Generic;
using System.Web.Mvc;
using Associativy.FrontendEngines.Controllers;
using Associativy.Models.Mind;
using Associativy.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using QuickGraph;
using Associativy.FrontendEngines.Shapes;
using Associativy.FrontendEngines.Services;

namespace Associativy.FrontendEngines.Engines.JIT.Controllers
{
    [OrchardFeature("Associativy")]
    public class JITController : FrontendEngineBaseController, IDiscoverableFrontendEngineController
    {
        protected override string FrontendEngine
        {
            get { return "JIT"; }
        }

        public JITController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory,
            IGraphFilterer graphFilterer)
            : base(associativyServices, orchardServices, frontendShapes, shapeFactory, graphFilterer)
        {
        }

        public virtual JsonResult FetchAssociations(int zoomLevel = 0)
        {
            var searchForm = _contentManager.New("AssociativySearchForm");
            _contentManager.UpdateEditor(searchForm, this);

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = zoomLevel;

            IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph;
            if (ModelState.IsValid)
            {
                if (!TryGetGraph(searchForm, out graph, settings, GraphQueryModifier))
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                graph = _mind.GetAllAssociations(settings, GraphQueryModifier);
            }

            var jsonData = new object[graph.VertexCount];


            var viewNodes = new Dictionary<int, ViewNode>(graph.VertexCount);

            foreach (var vertex in graph.Vertices)
            {
                viewNodes[vertex.Id] = new ViewNode
                {
                    id = vertex.Id.ToString(),
                    name = vertex.As<ITitleAspect>().Title,
                    adjacencies = new List<string>()
                };
            }

            foreach (var edge in graph.Edges)
            {
                viewNodes[edge.Source.Id].adjacencies.Add(edge.Target.Id.ToString());
                viewNodes[edge.Target.Id].adjacencies.Add(edge.Source.Id.ToString());
            }

            return Json(viewNodes.Values, JsonRequestBehavior.AllowGet);
        }

        protected class ViewNode
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<string> adjacencies { get; set; }
            public IDictionary<string, string> data { get; set; }
        }
    }
}