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
using Associativy.Services.Notions;

/*
 * Search().Parse(searchFields, query, false); kell a partial match-hez, wildcarddal (kere*), a false tiltja az escape-elést
 * Példa: Modules\Orchard.Search\Services\SearchService.cs
 */

namespace Associativy.Controllers
{
    public class TestController : Controller
    {
        private readonly IAssociativyNotionsServices _associativyService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<NotionToNotionConnectorRecord> _repository;

        public TestController(
            IAssociativyNotionsServices associativyService,
            IContentManager contentManager,
            IOrchardServices orchardServices,
            IRepository<NotionToNotionConnectorRecord> repository)
        {
            _associativyService = associativyService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;

            _repository = repository;
        }

        //
        // GET: /Test/

        public ActionResult Index()
        {
            var node = _contentManager.New<NotionPart>("Notion");
            node.Label = "kkK";
            _associativyService.NodeManager.Update(node);

            return null;
        }


        public ActionResult Create()
        {
            Dictionary<string, NotionPart> nodes = new Dictionary<string, NotionPart>();

            // new NotionParams helyett factory!
            nodes["jég"] = _associativyService.NodeManager.Create(new NotionParams { Label = "jég" });
            nodes["fagyott"] = _associativyService.NodeManager.Create(new NotionParams { Label = "fagyott" });
            nodes["fagylalt"] = _associativyService.NodeManager.Create(new NotionParams { Label = "fagylalt" });
            nodes["téli fagylalt"] = _associativyService.NodeManager.Create(new NotionParams { Label = "téli fagylalt" });
            nodes["tél"] = _associativyService.NodeManager.Create(new NotionParams { Label = "tél" });
            nodes["víz"] = _associativyService.NodeManager.Create(new NotionParams { Label = "víz" });
            nodes["nyár"] = _associativyService.NodeManager.Create(new NotionParams { Label = "nyár" });
            nodes["meleg"] = _associativyService.NodeManager.Create(new NotionParams { Label = "neleg" });
            nodes["forró"] = _associativyService.NodeManager.Create(new NotionParams { Label = "forró" });
            nodes["tűz"] = _associativyService.NodeManager.Create(new NotionParams { Label = "tűz" });
            nodes["tűzhely"] = _associativyService.NodeManager.Create(new NotionParams { Label = "tűzhely" });
            nodes["gőz"] = _associativyService.NodeManager.Create(new NotionParams { Label = "gőz" });
            nodes["oxigén"] = _associativyService.NodeManager.Create(new NotionParams { Label = "oxigén" });
            nodes["levegő"] = _associativyService.NodeManager.Create(new NotionParams { Label = "levegő" });
            nodes["folyó"] = _associativyService.NodeManager.Create(new NotionParams { Label = "folyó" });
            nodes["nitrogén"] = _associativyService.NodeManager.Create(new NotionParams { Label = "nitrogén" });
            nodes["hidrogén"] = _associativyService.NodeManager.Create(new NotionParams { Label = "hidrogén" });
            nodes["föld"] = _associativyService.NodeManager.Create(new NotionParams { Label = "föld" });
            nodes["út"] = _associativyService.NodeManager.Create(new NotionParams { Label = "út" });
            nodes["autó"] = _associativyService.NodeManager.Create(new NotionParams { Label = "autó" });
            nodes["téligumi"] = _associativyService.NodeManager.Create(new NotionParams { Label = "téligumi" });
            nodes["fagyálló"] = _associativyService.NodeManager.Create(new NotionParams { Label = "fagyálló" });
            nodes["sofőr"] = _associativyService.NodeManager.Create(new NotionParams { Label = "sofőr" });
            nodes["kerék"] = _associativyService.NodeManager.Create(new NotionParams { Label = "kerék" });
            nodes["Audi"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Audi" });
            nodes["Suzuki"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Suzuki" });
            nodes["Maserati"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Maserati" });
            nodes["BMW"] = _associativyService.NodeManager.Create(new NotionParams { Label = "BMW" });
            nodes["Mercedes"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Mercedes" });
            nodes["benzin"] = _associativyService.NodeManager.Create(new NotionParams { Label = "benzin" });
            nodes["Volkswagen"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Volkswagen" });
            nodes["Skoda"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Skoda" });
            nodes["Honda"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Honda" });
            nodes["márka"] = _associativyService.NodeManager.Create(new NotionParams { Label = "márka" });
            nodes["német"] = _associativyService.NodeManager.Create(new NotionParams { Label = "német" });
            nodes["kaiserlich und königlich"] = _associativyService.NodeManager.Create(new NotionParams { Label = "kaiserlich und königlich" });
            nodes["kuk"] = _associativyService.NodeManager.Create(new NotionParams { Label = "kuk" });
            nodes["mozaikszó"] = _associativyService.NodeManager.Create(new NotionParams { Label = "mozaikszó" });
            nodes["OSZK"] = _associativyService.NodeManager.Create(new NotionParams { Label = "OSZK" });
            nodes["Országos Széchenyi Könyvtár"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Országos Széchenyi Könyvtár" });
            nodes["gumi"] = _associativyService.NodeManager.Create(new NotionParams { Label = "gumi" });
            nodes["rágógumi"] = _associativyService.NodeManager.Create(new NotionParams { Label = "rágógumi" });
            nodes["kaucsuk"] = _associativyService.NodeManager.Create(new NotionParams { Label = "kaucsuk" });
            nodes["Japán"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Japán" });
            nodes["motorbicikli"] = _associativyService.NodeManager.Create(new NotionParams { Label = "motorbicikli" });
            nodes["mission accomplished"] = _associativyService.NodeManager.Create(new NotionParams { Label = "mission accomplished" });
            nodes["feladat befejezve"] = _associativyService.NodeManager.Create(new NotionParams { Label = "feladat befejezve" });
            nodes["Nap"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Nap" });
            nodes["sárga"] = _associativyService.NodeManager.Create(new NotionParams { Label = "sárga" });
            nodes["lila"] = _associativyService.NodeManager.Create(new NotionParams { Label = "lila" });
            nodes["orgona"] = _associativyService.NodeManager.Create(new NotionParams { Label = "orgona" });
            nodes["virág"] = _associativyService.NodeManager.Create(new NotionParams { Label = "virág" });
            nodes["szín"] = _associativyService.NodeManager.Create(new NotionParams { Label = "szín" });
            nodes["őselem"] = _associativyService.NodeManager.Create(new NotionParams { Label = "őselem" });
            nodes["Prométheusz"] = _associativyService.NodeManager.Create(new NotionParams { Label = "Prométheusz" });

            
            _associativyService.NodeManager.AddConnection(nodes["mission accomplished"], nodes["feladat befejezve"]);
            _associativyService.NodeManager.AddConnection(nodes["Nap"], nodes["sárga"]);
            _associativyService.NodeManager.AddConnection(nodes["lila"], nodes["orgona"]);
            _associativyService.NodeManager.AddConnection(nodes["orgona"], nodes["virág"]);
            _associativyService.NodeManager.AddConnection(nodes["szín"], nodes["lila"]);
            _associativyService.NodeManager.AddConnection(nodes["szín"], nodes["sárga"]);
            _associativyService.NodeManager.AddConnection(nodes["német"], nodes["Volkswagen"]);
            _associativyService.NodeManager.AddConnection(nodes["német"], nodes["BMW"]);
            _associativyService.NodeManager.AddConnection(nodes["német"], nodes["Audi"]);
            _associativyService.NodeManager.AddConnection(nodes["német"], nodes["Mercedes"]);
            _associativyService.NodeManager.AddConnection(nodes["német"], nodes["kaiserlich und königlich"]);
            _associativyService.NodeManager.AddConnection(nodes["kaiserlich und königlich"], nodes["kuk"]);
            _associativyService.NodeManager.AddConnection(nodes["mozaikszó"], nodes["kuk"]);
            _associativyService.NodeManager.AddConnection(nodes["mozaikszó"], nodes["BMW"]);
            _associativyService.NodeManager.AddConnection(nodes["mozaikszó"], nodes["OSZK"]);
            _associativyService.NodeManager.AddConnection(nodes["Országos Széchenyi Könyvtár"], nodes["OSZK"]);
            _associativyService.NodeManager.AddConnection(nodes["gumi"], nodes["téligumi"]);
            _associativyService.NodeManager.AddConnection(nodes["gumi"], nodes["kerék"]);
            _associativyService.NodeManager.AddConnection(nodes["gumi"], nodes["rágógumi"]);
            _associativyService.NodeManager.AddConnection(nodes["gumi"], nodes["kaucsuk"]);
            _associativyService.NodeManager.AddConnection(nodes["Honda"], nodes["Japán"]);
            _associativyService.NodeManager.AddConnection(nodes["Honda"], nodes["motorbicikli"]);
            _associativyService.NodeManager.AddConnection(nodes["Audi"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Suzuki"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Maserati"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["BMW"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Mercedes"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Volkswagen"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Skoda"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["Honda"], nodes["márka"]);
            _associativyService.NodeManager.AddConnection(nodes["téligumi"], nodes["autó"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["fagyálló"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["sofőr"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["kerék"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Suzuki"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Maserati"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Audi"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["BMW"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Mercedes"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["benzin"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Volkswagen"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Skoda"]);
            _associativyService.NodeManager.AddConnection(nodes["autó"], nodes["Honda"]);
            _associativyService.NodeManager.AddConnection(nodes["oxigén"], nodes["levegő"]);
            _associativyService.NodeManager.AddConnection(nodes["levegő"], nodes["őselem"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["őselem"]);
            _associativyService.NodeManager.AddConnection(nodes["folyó"], nodes["víz"]);
            _associativyService.NodeManager.AddConnection(nodes["nitrogén"], nodes["levegő"]);
            _associativyService.NodeManager.AddConnection(nodes["föld"], nodes["őselem"]);
            _associativyService.NodeManager.AddConnection(nodes["út"], nodes["föld"]);
            _associativyService.NodeManager.AddConnection(nodes["út"], nodes["autó"]);
            _associativyService.NodeManager.AddConnection(nodes["nyár"], nodes["meleg"]);
            _associativyService.NodeManager.AddConnection(nodes["nyár"], nodes["fagylalt"]);
            _associativyService.NodeManager.AddConnection(nodes["forró"], nodes["meleg"]);
            _associativyService.NodeManager.AddConnection(nodes["forró"], nodes["gőz"]);
            _associativyService.NodeManager.AddConnection(nodes["forró"], nodes["tűzhely"]);
            _associativyService.NodeManager.AddConnection(nodes["forró"], nodes["tűz"]);
            _associativyService.NodeManager.AddConnection(nodes["tűz"], nodes["tűzhely"]);
            _associativyService.NodeManager.AddConnection(nodes["tűz"], nodes["forró"]);
            _associativyService.NodeManager.AddConnection(nodes["tűz"], nodes["őselem"]);
            _associativyService.NodeManager.AddConnection(nodes["tűz"], nodes["Prométheusz"]);
            _associativyService.NodeManager.AddConnection(nodes["tűzhely"], nodes["gőz"]);
            _associativyService.NodeManager.AddConnection(nodes["jég"], nodes["fagyott"]);
            _associativyService.NodeManager.AddConnection(nodes["fagyott"], nodes["fagylalt"]);
            _associativyService.NodeManager.AddConnection(nodes["fagylalt"], nodes["jég"]);
            _associativyService.NodeManager.AddConnection(nodes["téli fagylalt"], nodes["fagylalt"]);
            _associativyService.NodeManager.AddConnection(nodes["téli fagylalt"], nodes["meleg"]);
            _associativyService.NodeManager.AddConnection(nodes["tél"], nodes["téli fagylalt"]);
            _associativyService.NodeManager.AddConnection(nodes["tél"], nodes["nyár"]);
            _associativyService.NodeManager.AddConnection(nodes["tél"], nodes["téligumi"]);
            _associativyService.NodeManager.AddConnection(nodes["tél"], nodes["fagyálló"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["jég"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["hidrogén"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["gőz"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["oxigén"]);
            _associativyService.NodeManager.AddConnection(nodes["víz"], nodes["folyó"]);



            return null;
        }

    }
}
