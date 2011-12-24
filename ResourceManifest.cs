using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("AssociativyAutoComplete").SetUrl("FrontendEngines/AssociativyAutoComplete.js").SetDependencies("jQueryUI_Autocomplete");

            // Dracula
            manifest.DefineScript("Raphael").SetUrl("FrontendEngines/Engines/Dracula/raphael-min.js");
            manifest.DefineScript("DraculaGraffle").SetUrl("FrontendEngines/Engines/Dracula/dracula_graffle.js");
            manifest.DefineScript("Dracula").SetUrl("FrontendEngines/Engines/Dracula/dracula_graph.js").SetDependencies(new string[] { "jQuery", "Raphael", "DraculaGraffle" });

            // JIT
            manifest.DefineScript("JITEngine").SetUrl("FrontendEngines/Engines/JIT/JIT.custom.min.js");
            manifest.DefineScript("JIT").SetUrl("FrontendEngines/Engines/JIT/JITDrawer.js").SetDependencies(new string[] { "jQuery", "JITEngine" });
        }
    }
}
