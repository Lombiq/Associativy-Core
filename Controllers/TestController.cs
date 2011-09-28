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
            int ssdfddfd;
            //var notion2 = _contentManager.New<NotionPart>("Notion");
            //notion2.Label = "tűzifa";
            //_contentManager.Create(notion2);

            //notion2 = _contentManager.New<NotionPart>("Notion");
            //notion2.Label = "tűzőgép";
            //_contentManager.Create(notion2);

            //var z = _notionAssociativyService.GetSimilarTerms("tű");
            //var notion1 = _contentManager.New<NotionPart>("Notion");
            //notion1.Label = "forró";
            //_contentManager.Create(notion1);

            //var notion2 = _contentManager.New<NotionPart>("Notion");
            //notion2.Label = "tűz";
            //_contentManager.Create(notion2);

            //_notionAssociativyService.AddConnection(notion1, notion2);


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

            //var z = _notionAssociativyService.GetNode(556565);
            //_notionAssociativyService.AddConnection(332626, 345355);
            //_notionAssociativyService.AddConnection(27, 66554665);

            _notionAssociativyService.DeleteNode(33);

            return null;
        }


        public ActionResult Create()
        {
            Dictionary<string, NotionPart> nodes = new Dictionary<string, NotionPart>();

            nodes["jég"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "jég" });
            nodes["fagyott"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "fagyott" });
            nodes["fagylalt"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "fagylalt" });
            nodes["téli fagylalt"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "téli fagylalt" });
            nodes["tél"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "tél" });
            nodes["víz"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "víz" });
            nodes["nyár"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "nyár" });
            nodes["meleg"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "neleg" });
            nodes["forró"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "forró" });
            nodes["tűz"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "tűz" });
            nodes["tűzhely"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "tűzhely" });
            nodes["gőz"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "gőz" });
            nodes["oxigén"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "oxigén" });
            nodes["levegő"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "levegő" });
            nodes["folyó"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "folyó" });
            nodes["nitrogén"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "nitrogén" });
            nodes["hidrogén"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "hidrogén" });
            nodes["föld"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "föld" });
            nodes["út"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "út" });
            nodes["autó"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "autó" });
            nodes["téligumi"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "téligumi" });
            nodes["fagyálló"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "fagyálló" });
            nodes["sofőr"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "sofőr" });
            nodes["kerék"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "kerék" });
            nodes["Audi"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Audi" });
            nodes["Suzuki"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Suzuki" });
            nodes["Maserati"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Maserati" });
            nodes["BMW"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "BMW" });
            nodes["Mercedes"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Mercedes" });
            nodes["benzin"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "benzin" });
            nodes["Volkswagen"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Volkswagen" });
            nodes["Skoda"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Skoda" });
            nodes["Honda"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Honda" });
            nodes["márka"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "márka" });
            nodes["német"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "német" });
            nodes["kaiserlich und königlich"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "kaiserlich und königlich" });
            nodes["kuk"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "kuk" });
            nodes["mozaikszó"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "mozaikszó" });
            nodes["OSZK"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "OSZK" });
            nodes["Országos Széchenyi Könyvtár"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Országos Széchenyi Könyvtár" });
            nodes["gumi"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "gumi" });
            nodes["rágógumi"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "rágógumi" });
            nodes["kaucsuk"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "kaucsuk" });
            nodes["Japán"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Japán" });
            nodes["motorbicikli"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "motorbicikli" });
            nodes["mission accomplished"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "mission accomplished" });
            nodes["feladat befejezve"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "feladat befejezve" });
            nodes["Nap"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Nap" });
            nodes["sárga"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "sárga" });
            nodes["lila"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "lila" });
            nodes["orgona"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "orgona" });
            nodes["virág"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "virág" });
            nodes["szín"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "szín" });
            nodes["őselem"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "őselem" });
            nodes["Prométheusz"] = _notionAssociativyService.CreateNode<NotionParams>(new NotionParams { Label = "Prométheusz" });

            _notionAssociativyService.AddConnection(nodes["mission accomplished"], nodes["feladat befejezve"]);
            _notionAssociativyService.AddConnection(nodes["Nap"], nodes["sárga"]);
            _notionAssociativyService.AddConnection(nodes["lila"], nodes["orgona"]);
            _notionAssociativyService.AddConnection(nodes["orgona"], nodes["virág"]);
            _notionAssociativyService.AddConnection(nodes["szín"], nodes["lila"]);
            _notionAssociativyService.AddConnection(nodes["szín"], nodes["sárga"]);
            _notionAssociativyService.AddConnection(nodes["német"], nodes["Volkswagen"]);
            _notionAssociativyService.AddConnection(nodes["német"], nodes["BMW"]);
            _notionAssociativyService.AddConnection(nodes["német"], nodes["Audi"]);
            _notionAssociativyService.AddConnection(nodes["német"], nodes["Mercedes"]);
            _notionAssociativyService.AddConnection(nodes["német"], nodes["kaiserlich und königlich"]);
            _notionAssociativyService.AddConnection(nodes["kaiserlich und königlich"], nodes["kuk"]);
            _notionAssociativyService.AddConnection(nodes["mozaikszó"], nodes["kuk"]);
            _notionAssociativyService.AddConnection(nodes["mozaikszó"], nodes["BMW"]);
            _notionAssociativyService.AddConnection(nodes["mozaikszó"], nodes["OSZK"]);
            _notionAssociativyService.AddConnection(nodes["Országos Széchenyi Könyvtár"], nodes["OSZK"]);
            _notionAssociativyService.AddConnection(nodes["gumi"], nodes["téligumi"]);
            _notionAssociativyService.AddConnection(nodes["gumi"], nodes["kerék"]);
            _notionAssociativyService.AddConnection(nodes["gumi"], nodes["rágógumi"]);
            _notionAssociativyService.AddConnection(nodes["gumi"], nodes["kaucsuk"]);
            _notionAssociativyService.AddConnection(nodes["Honda"], nodes["Japán"]);
            _notionAssociativyService.AddConnection(nodes["Honda"], nodes["motorbicikli"]);
            _notionAssociativyService.AddConnection(nodes["Audi"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Suzuki"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Maserati"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["BMW"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Mercedes"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Volkswagen"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Skoda"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["Honda"], nodes["márka"]);
            _notionAssociativyService.AddConnection(nodes["téligumi"], nodes["autó"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["fagyálló"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["sofőr"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["kerék"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Suzuki"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Maserati"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Audi"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["BMW"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Mercedes"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["benzin"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Volkswagen"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Skoda"]);
            _notionAssociativyService.AddConnection(nodes["autó"], nodes["Honda"]);
            _notionAssociativyService.AddConnection(nodes["oxigén"], nodes["levegő"]);
            _notionAssociativyService.AddConnection(nodes["levegő"], nodes["őselem"]);
            _notionAssociativyService.AddConnection(nodes["folyó"], nodes["víz"]);
            _notionAssociativyService.AddConnection(nodes["nitrogén"], nodes["levegő"]);
            _notionAssociativyService.AddConnection(nodes["föld"], nodes["őselem"]);
            _notionAssociativyService.AddConnection(nodes["út"], nodes["föld"]);
            _notionAssociativyService.AddConnection(nodes["út"], nodes["autó"]);
            _notionAssociativyService.AddConnection(nodes["nyár"], nodes["meleg"]);
            _notionAssociativyService.AddConnection(nodes["nyár"], nodes["fagylalt"]);
            _notionAssociativyService.AddConnection(nodes["forró"], nodes["meleg"]);
            _notionAssociativyService.AddConnection(nodes["forró"], nodes["gőz"]);
            _notionAssociativyService.AddConnection(nodes["forró"], nodes["tűzhely"]);
            _notionAssociativyService.AddConnection(nodes["forró"], nodes["tűz"]);
            _notionAssociativyService.AddConnection(nodes["tűz"], nodes["tűzhely"]);
            _notionAssociativyService.AddConnection(nodes["tűz"], nodes["forró"]);
            _notionAssociativyService.AddConnection(nodes["tűz"], nodes["őselem"]);
            _notionAssociativyService.AddConnection(nodes["tűz"], nodes["Prométheusz"]);
            _notionAssociativyService.AddConnection(nodes["tűzhely"], nodes["gőz"]);
            _notionAssociativyService.AddConnection(nodes["jég"], nodes["fagyott"]);
            _notionAssociativyService.AddConnection(nodes["fagyott"], nodes["fagylalt"]);
            _notionAssociativyService.AddConnection(nodes["fagylalt"], nodes["jég"]);
            _notionAssociativyService.AddConnection(nodes["téli fagylalt"], nodes["fagylalt"]);
            _notionAssociativyService.AddConnection(nodes["téli fagylalt"], nodes["meleg"]);
            _notionAssociativyService.AddConnection(nodes["tél"], nodes["téli fagylalt"]);
            _notionAssociativyService.AddConnection(nodes["tél"], nodes["nyár"]);
            _notionAssociativyService.AddConnection(nodes["tél"], nodes["téligumi"]);
            _notionAssociativyService.AddConnection(nodes["tél"], nodes["fagyálló"]);
            _notionAssociativyService.AddConnection(nodes["víz"], nodes["jég"]);
            _notionAssociativyService.AddConnection(nodes["víz"], nodes["hidrogén"]);
            _notionAssociativyService.AddConnection(nodes["víz"], nodes["gőz"]);
            _notionAssociativyService.AddConnection(nodes["víz"], nodes["oxigén"]);
            _notionAssociativyService.AddConnection(nodes["víz"], nodes["folyó"]);



            return null;
        }

    }
}
