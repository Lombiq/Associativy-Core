using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Associativy.FrontendEngines.Engines.Graphviz
{
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("OpenLayers").SetUrl("FrontendEngines/Engines/Graphviz/OpenLayers-2.11/OpenLayers.js").SetVersion("2.11");
            manifest.DefineScript("MapQuery").SetUrl("FrontendEngines/Engines/Graphviz/MapQuery-0.1/src/jquery.mapquery.core.js").SetVersion("0.1").SetDependencies(new string[] { "OpenLayers" });
            manifest.DefineScript("MapQuery.ImageLayer").SetUrl("FrontendEngines/Engines/Graphviz/jquery.mapquery.imageLayer.js").SetDependencies(new string[] { "MapQuery" });
            manifest.DefineScript("Graphviz").SetDependencies(new string[] { "MapQuery" });
        }
    }
}
