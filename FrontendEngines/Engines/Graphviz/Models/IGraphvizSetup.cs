﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.FrontendEngines.Models;
using Orchard.ContentManagement;
using QuickGraph.Graphviz;

namespace Associativy.FrontendEngines.Engines.Graphviz.Models
{
    public interface IGraphvizSetup : IFrontendEngineSetup
    {
        FormatVertexEventHandler<IContent> VertexFormatter { get; }
    }
}