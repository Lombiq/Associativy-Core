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

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Associativy.FrontendEngines.Controllers.AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IDraculaDriver<TNodePart> _draculaDriver;

        protected override string FrontendEngineDriver
        {
            get { return "Dracula"; }
        }

        protected AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices,
            IDraculaDriver<TNodePart> draculaDriver)
            : base(associativyService, orchardServices, draculaDriver)
        {
            _draculaDriver = draculaDriver;
        }
    }
}