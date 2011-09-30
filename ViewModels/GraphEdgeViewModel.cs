using Orchard.Environment.Extensions;

namespace Associativy.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphEdgeViewModel
    {
        public string SourceLabel { get; set; }
        public string TargetLabel { get; set; }
    }
}