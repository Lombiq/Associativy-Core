using Associativy.Models;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Handles registered contexts
    /// </summary>
    /// <remarks>
    /// Could be superfluous.
    /// </remarks>
    public interface IAssociativyContextLocator : IDependency
    {
        /// <summary>
        /// Returns the IAssociativyContext with the corresponding technical name, if it was registered
        /// </summary>
        /// <param name="technicalGraphName">The technical name of the context's graph</param>
        IAssociativyContext GetContext(string technicalGraphName);
    }
}
