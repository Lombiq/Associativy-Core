using System;
using System.Collections.Generic;
using Associativy.Models;
using Associativy.FrontendEngines.ViewModels;

namespace Associativy.FrontendEngines.Engines.JIT.ViewModels
{
    public interface IJITGraphNodeViewModel<TNode> : IGraphNodeViewModel<TNode>
        where TNode : INode
    {
        string id { get; set; }
        string name { get; set; }
        string[] adjacencies { get; set; }
        IDictionary<string, string> data { get; set; }
    }
}
