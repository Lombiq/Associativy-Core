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

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        : FrontendEngineBaseController<TNodeToNodeConnectorRecord, TAssociativyContext>, IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes shapes,
            IShapeFactory shapeFactory)
            : base(associativyServices, orchardServices, shapes, shapeFactory)
        {
        }
    }
}