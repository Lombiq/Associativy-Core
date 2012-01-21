using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Associativy.GraphDiscovery
{
    public interface IProviderFilterer : IDependency
    {
        IEnumerable<TAssociativyProvider> FilterByMatchingGraphContext<TAssociativyProvider>(
            IEnumerable<TAssociativyProvider> providers,
            IGraphContext graphContext,
            Func<TAssociativyProvider, string> graphNameSelector,
            Func<TAssociativyProvider, IEnumerable<string>> contentTypesSelector)
            where TAssociativyProvider : IAssociativyProvider;
    }
}
