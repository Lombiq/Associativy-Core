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

namespace Associativy.FrontendEngines.Engines.Graphviz.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        : FrontendEngineBaseController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, IDiscoverableFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IGraphvizDriver<TNodePart> _graphvizDriver;
        protected readonly IDetachedDelegateBuilder _detachedDelegateBuilder;
        protected readonly GraphImageService<TNodePart> _graphImageService;

        protected override string FrontendEngineDriver
        {
            get { return "Graphviz"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> associativyServices,
            IOrchardServices orchardServices,
            IGraphvizDriver<TNodePart> graphvizDriver,
            IDetachedDelegateBuilder detachedDelegateBuilder,
            IGraphImageService<TNodePart> graphImageService)
            : base(associativyServices, orchardServices, graphvizDriver)
        {
            _graphvizDriver = graphvizDriver;
            _detachedDelegateBuilder = detachedDelegateBuilder;
            _graphImageService = (GraphImageService<TNodePart>)graphImageService;
        }

        public void Index()
        {
            var count = 2;

            var sw = new Stopwatch();
            sw.Start();

            var graphs = new IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>[count];
            for (int i = 0; i < count; i++)
            {
                graphs[i] = _mind.GetAllAssociations();
            }

            sw.Stop();
            var z = sw.ElapsedMilliseconds;
            sw.Restart();

            var tasks = new Task<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>[count];
            for (int i = 0; i < count; i++)
            {
                tasks[i] = Task<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>.Factory.StartNew(
                    _detachedDelegateBuilder.BuildBackgroundFunction<IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>>>(
                        () => _mind.GetAllAssociations()
                    )
                    );
            }
            Task.WaitAll(tasks);

            sw.Stop();
            var y = sw.ElapsedMilliseconds;
            int ze = 5 + 5;
        }

        public virtual JsonResult Render(int zoomLevel = 0)
        {
            var searchViewModel = _frontendEngineDriver.GetSearchViewModel(this);

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = zoomLevel;

            
            IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph;
            if (ModelState.IsValid)
            {
                if (TryGetGraph(searchViewModel, out graph, settings))
                {
                    //jsonData = _jitDriver.GraphJson(graph);
                }
                else
                {
                    //jsonData = null;
                }
            }
            else
            {
                graph = _mind.GetAllAssociations(settings);
            }


            var graphImageUrl = _graphImageService.ToSvg(graph, algorithm =>
            {
                algorithm.FormatVertex +=
                    (sender, e) =>
                    {
                        e.VertexFormatter.Label = e.Vertex.Label;
                        e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Diamond;
                        e.VertexFormatter.Url = "http://pyrocenter.hu";
                    };
            });


            return Json(new { GraphImageUrl = graphImageUrl }, JsonRequestBehavior.AllowGet);
        }
    }
}