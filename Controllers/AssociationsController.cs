using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;
using Associativy.ViewModels.Notions;
using Orchard;
using Orchard.Mvc;
using Associativy.Services;
using Associativy.Models;

namespace Associativy.Controllers
{
    [Themed]
    public class AssociationsController : Controller
    {
        private readonly IAssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> _associativyService;
        private readonly IOrchardServices _orchardServices;

        private readonly AssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> _notionAssociativyService;

        public AssociationsController(
            IAssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> associativyService,
            IOrchardServices orchardServices)
        {
            _associativyService = associativyService;
            _orchardServices = orchardServices;

            _notionAssociativyService = associativyService as AssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord>;
        }

        public ActionResult ShowAssociations()
        {
            var viewModel = new NotionSearchViewModel();
            var z = TryUpdateModel<NotionSearchViewModel>(viewModel);
            if (ModelState.IsValid)
            {
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    //_notifier.Error(T(error));
                }

                return null;
            }

            //return new ShapeResult(this, _orchardServices.New.Graph()); 

            return null;
        }

        public JsonResult FetchSimilarTerms(string term)
        {
            var z = Json(_notionAssociativyService.GetSimilarTerms(term));
            return Json(_notionAssociativyService.GetSimilarTerms(term));
        }
    }
}
