using Orchard.UI.Resources;
using Orchard.Environment.Extensions;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineScript("AssociativyAutoComplete").SetUrl("AssociativyAutoComplete.js").SetDependencies("jQueryUI_Autocomplete");

            manifest.DefineScript("MooTools").SetCdn("https://ajax.googleapis.com/ajax/libs/mootools/1.4.0/mootools.js");
            manifest.DefineScript("Raphael").SetUrl("Dracula/raphael-min.js");
            manifest.DefineScript("DraculaGraffle").SetUrl("Dracula/dracula_graffle.js");
            manifest.DefineScript("Dracula").SetUrl("Dracula/dracula_graph.js").SetDependencies(new string[] { "jQuery", "Raphael", "DraculaGraffle" });
        }
    }
}
