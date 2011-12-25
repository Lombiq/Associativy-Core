using System;
using QuickGraph;
using Associativy.Models;

namespace Associativy.FrontendEngines.Engines.JIT
{
    public interface IJITDriver<TNode> : IFrontendEngineDriver<TNode>
        where TNode : INode
    {
        string GraphJson(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph);
    }
}
