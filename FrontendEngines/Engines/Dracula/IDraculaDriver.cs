using System;
using Associativy.Models;

namespace Associativy.FrontendEngines.Engines.Dracula
{
    public interface IDraculaDriver<TNode> : IFrontendEngineDriver<TNode>
        where TNode : INode
    {
    }
}
