using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Associativy.Services;
using Orchard.ContentManagement;
using Associativy.Models;
using Orchard.Data;
using QuickGraph;
using System.Diagnostics;
using System.Threading.Tasks;
using Orchard;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Autofac;
using Associativy.Models;

/*
 * Search().Parse(searchFields, query, false); kell a partial match-hez, wildcarddal (kere*), a false tiltja az escape-elést
 * Példa: Modules\Orchard.Search\Services\SearchService.cs
 */

namespace Associativy.Controllers
{
    public class TestController : Controller
    {
        private readonly IAssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> _associativyService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<NotionToNotionConnectorRecord> _repository;

        private readonly AssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> _notionAssociativyService;

        public TestController(
            IAssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord> associativyService,
            IContentManager contentManager,
            IOrchardServices orchardServices,
            IRepository<NotionToNotionConnectorRecord> repository)
        {
            _associativyService = associativyService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;

            _repository = repository;
            _notionAssociativyService = associativyService as AssociativyService<NotionPart, NotionPartRecord, NotionToNotionConnectorRecord>;
        }

        //
        // GET: /Test/

        public ActionResult Index()
        {
            var z = _notionAssociativyService.GetSimilarTerms("tű");
            var notion1 = _contentManager.New<NotionPart>("Notion");
            notion1.Label = "forró";
            _contentManager.Create(notion1);

            var notion2 = _contentManager.New<NotionPart>("Notion");
            notion2.Label = "tűz";
            _contentManager.Create(notion2);

            _notionAssociativyService.AddConnection(notion1, notion2);


            //var g = new UndirectedGraph<int, UndirectedEdge<int>>();

            //g.AddVerticesAndEdge(new UndirectedEdge<int>(1, 2));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(1, 3));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(1, 4));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(3, 2));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(2, 5));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(5, 6));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(5, 1));
            //g.AddVerticesAndEdge(new UndirectedEdge<int>(1, 7));
            //var y = g.Edges.ToList();
            //var u = g.ContainsEdge(1, 2);

            //var g = new BidirectionalGraph<int, Edge<int>>();

            //g.AddVerticesAndEdge(new Edge<int>(1, 2));
            //g.AddVerticesAndEdge(new Edge<int>(1, 3));
            //g.AddVerticesAndEdge(new Edge<int>(1, 4));
            //g.AddVerticesAndEdge(new Edge<int>(3, 2));
            //g.AddVerticesAndEdge(new Edge<int>(2, 5));
            //g.AddVerticesAndEdge(new Edge<int>(5, 6));
            //g.AddVerticesAndEdge(new Edge<int>(5, 1));
            //g.AddVerticesAndEdge(new Edge<int>(1, 7));
            //var y = g.Edges.ToList();
            //var u = g.ContainsEdge(2, 1);
            //var usd = g.ContainsEdge(1, 2);


            //var sw = new Stopwatch();
            //sw.Start();
            //for (int i = 0; i < 9999; i++)
            //{
            //    //((AssociativyService)_associativyService).GetNeighbourIds(i);
            //}
            //sw.Stop();
            //var x = sw.ElapsedMilliseconds;

            //var z = _notionAssociativyService.CalculatePaths(1, 3);

            return null;
        }

        public ActionResult Create()
        {

            return null;
        }

    }
}
