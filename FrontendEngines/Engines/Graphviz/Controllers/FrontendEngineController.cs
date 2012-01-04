using System.Web.Mvc;
using Associativy.FrontendEngines.Controllers;
using Associativy.FrontendEngines.Engines.Graphviz.Services;
using Associativy.Models;
using Associativy.Models.Mind;
using Associativy.Services;
using Associativy.Shapes;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.Tasks;
using QuickGraph;
using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace Associativy.FrontendEngines.Engines.Graphviz.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        : FrontendEngineBaseController, IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
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

        public virtual JsonResult Render()
        {
            var searchForm = _contentManager.New("AssociativySearchForm");
            _contentManager.UpdateEditor(searchForm, this);

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();

            List<string> graphImageUrls;

            if (ModelState.IsValid)
            {
                graphImageUrls = FetchZoomedGraphUrls(
                            settings,
                            (currentSettings) =>
                            {
                                IUndirectedGraph<IContent, IUndirectedEdge<IContent>> currentGraph;
                                TryGetGraph(searchForm, out currentGraph, settings, GraphQueryModifier);
                                return currentGraph;
                            });
            }
            else
            {
                graphImageUrls = FetchZoomedGraphUrls(
                            settings,
                            (currentSettings) =>
                            {
                                return _mind.GetAllAssociations(settings, GraphQueryModifier);
                            });
            }


            return Json(new { GraphImageUrls = graphImageUrls }, JsonRequestBehavior.AllowGet);
        }

        protected virtual List<string> FetchZoomedGraphUrls(IMindSettings settings, Func<IMindSettings, IUndirectedGraph<IContent, IUndirectedEdge<IContent>>> fetchGraph)
        {
            var graphImageUrls = new List<string>(_associativyServices.Context.MaxZoomLevel);

            Func<int, string> getImageUrl =
                (zoomLevel) =>
                {
                    settings.ZoomLevel = zoomLevel;
                    return _graphImageService.ToSvg(fetchGraph(settings), algorithm =>
                            {
                                algorithm.FormatVertex +=
                                    (sender, e) =>
                                    {
                                        e.VertexFormatter.Label = e.Vertex.As<ITitleAspect>().Title;
                                        e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Diamond;
                                        e.VertexFormatter.Url = e.Vertex.As<IRoutableAspect>().Path;
                                    };
                            });
                };

            graphImageUrls.Add(getImageUrl(0));

            var currentImageUrl = getImageUrl(1);
            int i = 1;
            while (i < _associativyServices.Context.MaxZoomLevel && graphImageUrls[i - 1] != currentImageUrl)
            {
                graphImageUrls.Add(currentImageUrl);
                i++;
                currentImageUrl = getImageUrl(i);
            }

            return graphImageUrls;
        }
    }
}