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
    public abstract class AssociativyBaseController : Controller
    {
        protected readonly IAssociativyServices _associativyServices;
        protected readonly IConnectionManager _connectionManager;
        protected readonly IMind _mind;
        protected readonly INodeManager _nodeManager;

        protected AssociativyBaseController(
            IAssociativyServices associativyServices)
        {
            _associativyServices = associativyServices;
            _connectionManager = associativyServices.ConnectionManager;
            _mind = associativyServices.Mind;
            _nodeManager = associativyServices.NodeManager;
        }
    }
}
