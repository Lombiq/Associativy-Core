using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Admin;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Associativy.Models;
using Orchard;
using Orchard.Core.Contents;

namespace Associativy.Controllers
{
    [Admin]
    public class AssociativyNodeLabelAdmin : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;

        public AssociativyNodeLabelAdmin(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _contentManager = orchardServices.ContentManager;
        }

        [HttpPost]
        public void RefreshLabels(string contentType)
        {
            var contentItems = _contentManager.Query(contentType).List();

            // If one's permitted to edit an item of this type then she/he is also permitted to refresh the labels...
            if (!_orchardServices.Authorizer.Authorize(Permissions.EditContent, contentItems.First())) return;

            foreach (var item in contentItems)
            {
                item.As<IAssociativyNodeLabelAspect>().Label = "";
                // This unpublish-publish fun is needed for the handler code to run. Otherwise, without the usage of an editor and calling UpdateEditor
                // there seems to be no way to invoke a content event when a content part was modified directly like above.
                _contentManager.Unpublish(item);
                _contentManager.Publish(item);
            }

            _contentManager.Flush();
        }
    }
}