using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using QuickGraph;
using System.Web.Mvc;
using Associativy.Models;
using Associativy.Models.Mind;
using Associativy.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.FrontendEngines.Controllers;
using Orchard.DisplayManagement;
using Orchard.ContentManagement.Aspects;

namespace Associativy.FrontendEngines.Engines.JIT.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        : FrontendEngineBaseController<TNodeToNodeConnectorRecord, TAssociativyContext>, IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected override string FrontendEngine
        {
            get { return "JIT"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> associativyServices,
            IOrchardServices orchardServices,
            IShapes shapes,
            IShapeFactory shapeFactory)
            : base(associativyServices, orchardServices, shapes, shapeFactory)
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