using Orchard.UI.Resources;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite {
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineScript("AssociativyAutoComplete").SetUrl("AssociativyAutoComplete.js").SetDependencies("jQueryUI_Autocomplete");
            //manifest.DefineStyle("FacebookConnect").SetUrl("FacebookConnect.css");
        }
    }
}
