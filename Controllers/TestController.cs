using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Associativy.Services;

/*
 * Search().Parse(searchFields, query, false); kell a partial match-hez, wildcarddal (kere*), a false tiltja az escape-elést
 * Példa: Modules\Orchard.Search\Services\SearchService.cs
 */

namespace Associativy.Controllers
{
    public class TestController : Controller
    {
        private readonly IAssociativyService _associativyService;

        public TestController(IAssociativyService associativyService)
        {
            _associativyService = associativyService;
        }

        //
        // GET: /Test/

        public ActionResult Index()
        {

            return null;
        }

    }
}
