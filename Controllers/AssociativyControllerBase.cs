using System.Web.Mvc;
using Associativy.GraphDiscovery;
using Associativy.Services;

namespace Associativy.Controllers
{
    public abstract class AssociativyControllerBase : Controller
    {
        protected readonly IAssociativyServices _associativyServices;
        protected readonly IGraphManager _graphManager;
        protected readonly IGraphEditor _graphEditor;


        protected AssociativyControllerBase(
            IAssociativyServices associativyServices)
        {
            _associativyServices = associativyServices;
            _graphManager = associativyServices.GraphManager;
            _graphEditor = associativyServices.GraphEditor;
        }
    }
}
