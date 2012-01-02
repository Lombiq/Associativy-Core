using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Associativy.Services;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines.Controllers
{
    /// <summary>
    /// Marker interface for frontend engine controllers that are auto-discovered and dispatched to from FrontendEngineDispatcherController
    /// </summary>
    public interface IDiscoverableFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IFrontendEngineController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, IDependency
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
    }
}
