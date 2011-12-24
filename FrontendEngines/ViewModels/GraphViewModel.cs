﻿using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphViewModel: IGraphViewModel
    {
        public IGraphSettings Settings { get; set; }
        public Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}