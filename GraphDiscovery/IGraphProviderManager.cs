using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Handles registered graphDescriptors
    /// </summary>
    public interface IGraphProviderManager : IDependency
    {
        IGraphProvider FindProvider(IGraphContext graphContext);

        bool TryFindProvidersForContentType(IContentContext contentContext, out IEnumerable<IGraphProvider> graphProviders);
    }
}
