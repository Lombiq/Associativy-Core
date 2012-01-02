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
using QuickGraph.Graphviz;
using System.Diagnostics;
using Piedone.HelpfulLibraries.Tasks;
using System.Threading.Tasks;
using Associativy.FrontendEngines.Controllers;
using Associativy.FrontendEngines.Engines.Graphviz.Services;
using QuickGraph.Data;
using Orchard.DisplayManagement;
using Associativy.Shapes;
using Orchard.ContentManagement.Aspects;

namespace Associativy.FrontendEngines.Engines.Graphviz.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        : FrontendEngineBaseController<TNodeToNodeConnectorRecord, TAssociativyContext>, IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected readonly IDetachedDelegateBuilder _detachedDelegateBuilder;
        protected readonly IGraphImageService<TAssociativyContext> _graphImageService;

        protected override string FrontendEngine
        {
            get { return "Graphviz"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes shapes,
            IShapeFactory shapeFactory,
            IDetachedDelegateBuilder detachedDelegateBuilder,
            IGraphImageService<TAssociativyContext> graphImageService)
            : base(associativyServices, orchardServices, shapes, shapeFactory)
        {
            _detachedDelegateBuilder = detachedDelegateBuilder;
            _graphImageService = graphImageService;
        }

        //public void Index()
        //{
        //    var count = 2;

        //    var sw = new Stopwatch();
        //    sw.Start();

        //    var graphs = new IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        graphs[i] = _mind.GetAllAssociations();
        //    }

        //    sw.Stop();
        //    var z = sw.ElapsedMilliseconds;
        //    sw.Restart();

        //    var tasks = new Task<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        tasks[i] = Task<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>.Factory.StartNew(
        //            _detachedDelegateBuilder.BuildBackgroundFunction<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>(
        //                () => _mind.GetAllAssociations()
        //            )
        //            );
        //    }
        //    Task.WaitAll(tasks);

        //    sw.Stop();
        //    var y = sw.ElapsedMilliseconds;
        //    int ze = 5 + 5;
        //}

        public virtual JsonResult Render(int zoomLevel = 0)
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
                    return null;
                }
            }
            else
            {
                graph = _mind.GetAllAssociations(settings, GraphQueryModifier);
            }


            var graphImageUrl = _graphImageService.ToSvg(graph, algorithm =>
            {
                algorithm.FormatVertex +=
                    (sender, e) =>
                    {
                        e.VertexFormatter.Label = e.Vertex.As<ITitleAspect>().Title;
                        e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Diamond;
                        e.VertexFormatter.Url = "http://pyrocenter.hu";
                    };
            });


            return Json(new { GraphImageUrl = graphImageUrl }, JsonRequestBehavior.AllowGet);
        }
    }
}