using System.Collections.Generic;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphResultViewModel: IGraphResultViewModel
    {
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}