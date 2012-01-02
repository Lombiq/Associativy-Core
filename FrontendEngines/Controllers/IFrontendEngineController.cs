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
    public interface IFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext> : IController
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
    }
}
