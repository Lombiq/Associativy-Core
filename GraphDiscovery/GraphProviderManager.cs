using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphProviderManager : IGraphProviderManager
    {
        private readonly IEnumerable<IGraphProvider> _registeredDescriptors;
        private readonly Dictionary<string, IGraphProvider> _providersByGraphName = new Dictionary<string, IGraphProvider>();
        private readonly Dictionary<string, List<IGraphProvider>> _providersByContentType = new Dictionary<string, List<IGraphProvider>>();

        public GraphProviderManager(IEnumerable<IGraphProvider> registeredDescriptors)
        {
            _registeredDescriptors = registeredDescriptors;
            CompileDescriptors(); // Maybe should be called elsewhere...
        }

        private void CompileDescriptors()
        {
            foreach (var provider in _registeredDescriptors)
            {
                _providersByGraphName[provider.GraphName] = provider; // Last one wins

                foreach (var contentType in provider.ContentTypes)
                {
                    if (!_providersByContentType.ContainsKey(contentType)) _providersByContentType[contentType] = new List<IGraphProvider>();
                    _providersByContentType[contentType].Add(provider);
                }
            }
        }

        public IGraphProvider FindProvider(IGraphContext graphContext)
        {
            IGraphProvider graphProvider;

            if (!_providersByGraphName.TryGetValue(graphContext.ProviderName, out graphProvider))
            {
                throw new ApplicationException("No graph provider found for the graph " + graphContext.ProviderName + ".");
            }

            return graphProvider;
        }

        public bool TryFindProvidersForContentType(IContentContext contentContext, out IEnumerable<IGraphProvider> graphProviders)
        {
            List<IGraphProvider> graphDescriptorList;

            var wasFound = _providersByContentType.TryGetValue(contentContext.ContentType, out graphDescriptorList);
            graphProviders = graphDescriptorList;

            return wasFound;
        }
    }
}