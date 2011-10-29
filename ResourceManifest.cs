using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineScript("AssociativyAutoComplete").SetUrl("AssociativyAutoComplete.js").SetDependencies("jQueryUI_Autocomplete");
            manifest.DefineScript("Raphael").SetUrl("Dracula/raphael-min.js");
            manifest.DefineScript("DraculaGraffle").SetUrl("Dracula/dracula_graffle.js");
            manifest.DefineScript("Dracula").SetUrl("Dracula/dracula_graph.js").SetDependencies(new string[] { "jQuery", "Raphael", "DraculaGraffle" });
        }
    }
}
