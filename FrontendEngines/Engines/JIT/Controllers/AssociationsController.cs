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

namespace Associativy.FrontendEngines.Engines.JIT.Controllers
{
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Associativy.FrontendEngines.Controllers.AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IJITDriver<TNodePart> _jitDriver;

        protected override string FrontendEngineDriver
        {
            get { return "JIT"; }
        }

        protected AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices,
            IJITDriver<TNodePart> jitDriver)
            : base(associativyService, orchardServices, jitDriver)
        {
            _jitDriver = jitDriver;
        }

        // No performance loss if with the same params as ShowAssociations because the solution 
        // is cached after ShowAssociations()
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
                jsonData = _jitDriver.GraphJson(_associativyServices.Mind.GetAllAssociations(settings));
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