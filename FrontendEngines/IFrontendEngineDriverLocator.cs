using System;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines
{
    /// <summary>
    /// Manages frontend engine (like Dracula or JIT) driver implementations
    /// </summary>
    /// <typeparam name="TNode">Node type</typeparam>
    public interface IFrontendEngineDriverLocator<TNode> : IDependency
        where TNode : INode
    {
        /// <summary>
        /// Returns the frontend engine driver specified by its name
        /// </summary>
        /// <param name="frontendEngineName">The frontend engine's name</param>
        /// <returns>The frontend engine driver</returns>
        IFrontendEngineDriver<TNode> GetDriver(string frontendEngineName);
    }
}
