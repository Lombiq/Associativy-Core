using System.Collections.Generic;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphViewModel: IGraphViewModel
    {
        public int ZoomLevel { get; set; }
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}