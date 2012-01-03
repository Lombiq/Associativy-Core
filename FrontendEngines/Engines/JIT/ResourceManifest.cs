using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Associativy.FrontendEngines.Engines.JIT
{
    [OrchardFeature("Associativy")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("JITEngine").SetUrl("FrontendEngines/Engines/JIT/JIT.custom.min.js");
            manifest.DefineScript("JIT").SetUrl("FrontendEngines/Engines/JIT/JITDrawer.js").SetDependencies(new string[] { "jQuery", "JITEngine" });
        }
    }
}
