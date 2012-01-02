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
    public abstract class AssociativyBaseController<TNodeToNodeConnectorRecord, TAssociativyContext> : Controller
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected readonly IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> _associativyServices;
        protected readonly IConnectionManager<TNodeToNodeConnectorRecord, TAssociativyContext> _connectionManager;
        protected readonly IMind<TNodeToNodeConnectorRecord, TAssociativyContext> _mind;
        protected readonly INodeManager<TAssociativyContext> _nodeManager;

        protected AssociativyBaseController(
            IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> associativyServices)
        {
            _associativyServices = associativyServices;
            _connectionManager = associativyServices.ConnectionManager;
            _mind = associativyServices.Mind;
            _nodeManager = associativyServices.NodeManager;
        }
    }
}
