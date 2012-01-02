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

namespace Associativy.FrontendEngines.Engines.JIT.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController< TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        : FrontendEngineBaseController< TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, IDiscoverableFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IJITDriver<TNodePart> _jitDriver;

        protected override string FrontendEngineDriver
        {
            get { return "JIT"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> associativyServices,
            IOrchardServices orchardServices,
            IJITDriver<TNodePart> jitDriver)
            : base(associativyServices, orchardServices, jitDriver)
        {
            _jitDriver = jitDriver;
        }

        public virtual JsonResult FetchAssociations(int zoomLevel = 0)
        {
            object jsonData = null;
            var searchViewModel = _frontendEngineDriver.GetSearchViewModel(this);

            var settings = _orchardServices.WorkContext.Resolve<IMindSettings>();
            settings.ZoomLevel = zoomLevel;

            if (ModelState.IsValid)
            {
                IUndirectedGraph<TNodePart, IUndirectedEdge<TNodePart>> graph;
                if (TryGetGraph(searchViewModel, out graph, settings))
                {
                    jsonData = _jitDriver.GraphJson(graph);
                }
                else
                {
                    jsonData = null;
                }
            }
            else
            {
                jsonData = _jitDriver.GraphJson(_mind.GetAllAssociations(settings));
            }

            var json = new JsonResult()
            {
                Data = jsonData,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return json;
        }
    }
}