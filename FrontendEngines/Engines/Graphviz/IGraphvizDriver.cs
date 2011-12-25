using System;
using Associativy.Models;

namespace Associativy.FrontendEngines.Engines.Graphviz
{
    public interface IGraphvizDriver<TNode> : IFrontendEngineDriver<TNode>
        where TNode : INode
    {
    }
}
