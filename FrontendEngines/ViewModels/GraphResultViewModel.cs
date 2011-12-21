using System.Collections.Generic;

namespace Associativy.FrontendEngines.ViewModels
{
    public class GraphResultViewModel: IGraphResultViewModel
    {
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}