using System;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines
{
    public interface IFrontendEngineDriverLocator<TNode> : IDependency
        where TNode : INode
    {
        IFrontendEngineDriver<TNode> GetDriver(string frontendEngineName);
    }
}
