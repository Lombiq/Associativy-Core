using System.Web.Mvc;
using Associativy.Services;

namespace Associativy.Controllers
{
    public abstract class AssociativyControllerBase : Controller
    {
        protected readonly IAssociativyServices _associativyServices;
        protected readonly IConnectionManager _connectionManager;
        protected readonly IMind _mind;
        protected readonly INodeManager _nodeManager;

        protected AssociativyControllerBase(
            IAssociativyServices associativyServices)
        {
            _associativyServices = associativyServices;
            _connectionManager = associativyServices.ConnectionManager;
            _mind = associativyServices.Mind;
            _nodeManager = associativyServices.NodeManager;
        }
    }
}
