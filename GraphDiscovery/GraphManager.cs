using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;
using System.Diagnostics;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphManager : IGraphManager
    {
        private readonly IEnumerable<IGraphProvider> _registeredProviders;
        private readonly List<GraphDescriptorImpl> _graphDescriptors = new List<GraphDescriptorImpl>();

        public GraphManager(IEnumerable<IGraphProvider> registeredProviders)
        {
            _registeredProviders = registeredProviders;
            CompileProviders(); // Maybe should be called elsewhere, lazily...
        }

        private void CompileProviders()
        {
            // Not so fast (1/2 ms), so should be lazy instead
            foreach (var provider in _registeredProviders)
            {
                var descriptor = new GraphDescriptorImpl();
                provider.Describe(descriptor);
                _graphDescriptors.Add(descriptor);
            }
        }

        public GraphDescriptor FindLastDescriptor(IGraphContext graphContext)
        {
            return FindDescriptors(graphContext).Last();
        }

        public IEnumerable<GraphDescriptor> FindDescriptors(IGraphContext graphContext)
        {
            // That's very fast (few dozen ticks), so there's no point in caching anything.
            IEnumerable<GraphDescriptorImpl> descriptors = _graphDescriptors;

            if (!String.IsNullOrEmpty(graphContext.GraphName))
            {
                descriptors = descriptors.Where((descriptor) => descriptor.GraphName == graphContext.GraphName);
            }

            if (graphContext.ContentTypes != null && graphContext.ContentTypes.Count() != 0)
            {
                descriptors = descriptors.Where((descriptor) =>
                    {
                        // A desciptor is only suitable if it contains all content types listed in the context
                        foreach (var contentType in graphContext.ContentTypes)
                        {
                            if (!descriptor.ContentTypes.Contains(contentType)) return false;
                        }

                        return true;
                    });
            }

            return descriptors;
        }


        private class GraphDescriptorImpl : GraphDescriptor
        {
        }
    }
}