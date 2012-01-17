using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.Services
{
    /// <summary>
    /// Handles registered graphDescriptors
    /// </summary>
    public interface IGraphDescriptorLocator : IDependency
    {
        /// <summary>
        /// Returns the IAssociativyGraphDescriptor with the corresponding technical name, if it was registered
        /// </summary>
        /// <param name="technicalGraphName">The technical name of the graphDescriptor's graph</param>
        IGraphDescriptor FindGraphDescriptor(string technicalGraphName);

        IEnumerable<IGraphDescriptor> FindGraphDescriptorsForContentType(string contentType);

        IDictionary<string, IList<IGraphDescriptor>> FindGraphDescriptorsByRegisteredContentTypes();
    }
}
