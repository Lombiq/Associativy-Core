using System;
using System.Web.Mvc;
using Associativy.Services;
using Orchard;
using Orchard.Exceptions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Associativy.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly INodeIndexingService _nodeIndexingService;
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public AdminController(INodeIndexingService nodeIndexingService, IOrchardServices orchardServices)
        {
            _nodeIndexingService = nodeIndexingService;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        public ActionResult SetupIndexingForGraph(string graphName, string returnUrl)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("You're not allowed to manage node indexes.")))
                return new HttpUnauthorizedResult();

            try
            {
                _nodeIndexingService.SetupIndexingForGraph(graphName);
                _orchardServices.Notifier.Information(T("Indexing set up for the graph. Nodes will soon be available for fetching by label."));
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;

                Logger.Error(ex, "Indexing for the graph " + graphName + " couldn't be set up.");

                _orchardServices.Notifier.Error(T("The index for the graph can't be set up. Is an indexing implementation like Lucene properly running?"));
            }

            return this.RedirectLocal(returnUrl);
        }
    }
}