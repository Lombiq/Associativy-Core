using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.ViewModels
{
    public class GraphResultViewModel: IGraphResultViewModel
    {
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}