﻿using System.Web.Mvc;
using Associativy.Services;
using Associativy.GraphDiscovery;

namespace Associativy.Controllers
{
    public abstract class AssociativyControllerBase : Controller
    {
        protected readonly IAssociativyServices _associativyServices;
        protected readonly IGraphManager _graphManager;
        protected readonly IGraphService _graphService;
        protected readonly IMind _mind;
        protected readonly INodeManager _nodeManager;

        protected AssociativyControllerBase(
            IAssociativyServices associativyServices)
        {
            _associativyServices = associativyServices;
            _graphManager = associativyServices.GraphManager;
            _graphService = associativyServices.GraphService;
            _mind = associativyServices.Mind;
            _nodeManager = associativyServices.NodeManager;
        }
    }
}
