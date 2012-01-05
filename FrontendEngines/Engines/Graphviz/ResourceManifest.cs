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

            // OpenLayers
            manifest.DefineScript("OpenLayers").SetUrl("FrontendEngines/Engines/Graphviz/OpenLayers-2.11/OpenLayers.js").SetVersion("2.11");
            manifest.DefineScript("MapQuery").SetUrl("FrontendEngines/Engines/Graphviz/MapQuery-0.1/src/jquery.mapquery.core.js").SetVersion("0.1").SetDependencies(new string[] { "OpenLayers", "jQuery" });
            manifest.DefineScript("MapQuery.ImageLayer").SetUrl("FrontendEngines/Engines/Graphviz/jquery.mapquery.imageLayer.js").SetDependencies(new string[] { "MapQuery" });
            
            // Mapz
            manifest.DefineScript("Mousewheel").SetUrl("FrontendEngines/Engines/Graphviz/jquery.mousewheel.js").SetDependencies(new string[] { "jQuery" });
            manifest.DefineScript("Mapz").SetUrl("FrontendEngines/Engines/Graphviz/jquery.mapz.js").SetDependencies(new string[] { "jQueryUI", "Mousewheel" });
            
            manifest.DefineScript("Graphviz").SetDependencies(new string[] { "Mapz" });
            manifest.DefineStyle("Graphviz").SetUrl("FrontendEngines/Engines/Graphviz/associativy-graphviz-styles.css");
        }
    }
}
