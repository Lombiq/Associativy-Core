﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickGraph;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public abstract class GraphViewModelBase : IGraphViewModel
    {
        public IUndirectedGraph<IContent, IUndirectedEdge<IContent>> Graph { get; set; }
    }
}