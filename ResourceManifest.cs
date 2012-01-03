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
        }
    }
}
