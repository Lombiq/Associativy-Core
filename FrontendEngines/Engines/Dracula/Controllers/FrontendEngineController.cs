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

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> 
        : FrontendEngineBaseController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, IDiscoverableFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IDraculaDriver<TNodePart> _draculaDriver;

        protected override string FrontendEngineDriver
        {
            get { return "Dracula"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> associativyService,
            IOrchardServices orchardServices,
            IDraculaDriver<TNodePart> draculaDriver)
            : base(associativyService, orchardServices, draculaDriver)
        {
            _draculaDriver = draculaDriver;
        }
    }
}