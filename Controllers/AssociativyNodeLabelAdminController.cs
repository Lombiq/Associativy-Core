using System.Linq;
using System.Web.Mvc;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Associativy.Controllers
{
    [Admin, OrchardFeature("Associativy")]
    public class AssociativyNodeLabelAdminController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;

        public Localizer T { get; set; }

        public AssociativyNodeLabelAdminController(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _contentManager = orchardServices.ContentManager;

            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public void RefreshLabels(string contentType)
        {
            var contentItems = _contentManager.Query(contentType).List();

            // If one's permitted to edit an item of this type then she/he is also permitted to refresh the labels...
            if (!_orchardServices.Authorizer.Authorize(Permissions.EditContent, contentItems.First())) return;

            foreach (var item in contentItems)
            {
                item.As<AssociativyNodeLabelPart>().Label = "";
                // This unpublish-publish fun is needed for the handler code to run. Otherwise, without the usage of an editor and calling UpdateEditor
                // there seems to be no way to invoke a content event when a content part was modified directly like above.
                _contentManager.Unpublish(item);
                _contentManager.Publish(item);
            }

            _contentManager.Flush();

            _orchardServices.Notifier.Information(T("Labels were refreshed."));
        }
    }
}