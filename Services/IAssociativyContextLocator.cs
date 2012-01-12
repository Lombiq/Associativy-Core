using Associativy.Models;
using Orchard;
using System.Collections.Generic;

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

        IAssociativyContext[] GetContextsForContentType(string contentType);

        IDictionary<string, IList<IAssociativyContext>> GetContextsByRegisteredContentTypes();
    }
}
