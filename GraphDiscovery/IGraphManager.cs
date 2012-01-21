using Associativy.Models;
using Orchard;
using System.Collections.Generic;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Handles registered graphs
    /// </summary>
    public interface IGraphManager : IDependency
    {
        IGraphProvider FindLastProvider(IGraphContext graphContext);

        IEnumerable<IGraphProvider> FindProviders(IGraphContext graphContext);
    }
}
