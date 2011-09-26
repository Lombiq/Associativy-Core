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

/*
 * Search().Parse(searchFields, query, false); kell a partial match-hez, wildcarddal (kere*), a false tiltja az escape-elést
 * Példa: Modules\Orchard.Search\Services\SearchService.cs
 */

namespace Associativy.Controllers
{
    public class TestController : Controller
    {
        private readonly IAssociativyService _associativyService;
        private readonly IContentManager _contentManager;
        private readonly IRepository<NodePartRecord> _nodePartRecordRepository;

        public TestController(
            IAssociativyService associativyService,
            IContentManager contentManager,
            IRepository<NodePartRecord> nodePartRecordRepository)
        {
            _associativyService = associativyService;
            _contentManager = contentManager;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        //
        // GET: /Test/

        public ActionResult Index()
        {
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

            var g = new BidirectionalGraph<int, Edge<int>>();

            g.AddVerticesAndEdge(new Edge<int>(1, 2));
            g.AddVerticesAndEdge(new Edge<int>(1, 3));
            g.AddVerticesAndEdge(new Edge<int>(1, 4));
            g.AddVerticesAndEdge(new Edge<int>(3, 2));
            g.AddVerticesAndEdge(new Edge<int>(2, 5));
            g.AddVerticesAndEdge(new Edge<int>(5, 6));
            g.AddVerticesAndEdge(new Edge<int>(5, 1));
            g.AddVerticesAndEdge(new Edge<int>(1, 7));
            var y = g.Edges.ToList();
            var u = g.ContainsEdge(2, 1);
            var usd = g.ContainsEdge(1, 2);


            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 9999; i++)
            {
                //((AssociativyService)_associativyService).GetNeighbourIds(i);
            }
            sw.Stop();
            var x = sw.ElapsedMilliseconds;

            var z = ((AssociativyService)_associativyService).CalcPaths(1, 3);
            var succeededNodeIds = new List<int>();
            //foreach (var item in z)
            //{
            //    succeededNodeIds = succeededNodeIds.Union(item).ToList();
            //}
            z.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList());

            //var z = _nodePartRecordRepository.Get(node => node.Id == 1);
            //var z = _contentManager.Query<NodePart, NodePartRecord>().Where(node => node.Id == 1).List().FirstOrDefault<NodePart>();
            return null;
        }

        public ActionResult Create()
        {

            return null;
        }

    }
}
