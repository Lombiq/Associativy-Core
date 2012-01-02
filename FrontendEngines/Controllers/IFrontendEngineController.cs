using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using Associativy.Services;
using System.Web.Mvc;

namespace Associativy.FrontendEngines.Controllers
{
    public interface IFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IController
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
    }
}
