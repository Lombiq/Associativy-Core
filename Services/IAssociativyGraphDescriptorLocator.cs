using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.Services
{
    /// <summary>
    /// Handles registered graphDescriptors
    /// </summary>
    public interface IAssociativyGraphDescriptorLocator : IDependency
    {
        /// <summary>
        /// Returns the IAssociativyGraphDescriptor with the corresponding technical name, if it was registered
        /// </summary>
        /// <param name="technicalGraphName">The technical name of the graphDescriptor's graph</param>
        IAssociativyGraphDescriptor FindGraphDescriptor(string technicalGraphName);

        IAssociativyGraphDescriptor[] FindGraphDescriptorsForContentType(string contentType);

        IDictionary<string, IList<IAssociativyGraphDescriptor>> FindGraphDescriptorsByRegisteredContentTypes();
    }
}
