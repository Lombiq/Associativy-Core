using System.Collections.Generic;

namespace Associativy.ViewModels
{
    public class GraphResultViewModel: IGraphResultViewModel
    {
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}