using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Associativy.Services;
using Orchard.ContentManagement;
using Associativy.Models;
using Orchard.ContentManagement.Records;

namespace Associativy.Controllers
{
    public abstract class AssociativyBaseController<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Controller
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _associativyServices;
        protected readonly IConnectionManager<TNodeToNodeConnectorRecord> _connectionManager;
        protected readonly IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _mind;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;

        protected AssociativyBaseController(
            IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> associativyServices)
        {
            _associativyServices = associativyServices;
            _connectionManager = associativyServices.ConnectionManager;
            _mind = associativyServices.Mind;
            _nodeManager = associativyServices.NodeManager;
        }
    }
}
