using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class RegisteredGraphDescriptorProvider : IGraphDescriptorProvider
    {
        private readonly IEnumerable<IGraphProvider> _registeredProviders;

        public RegisteredGraphDescriptorProvider(IEnumerable<IGraphProvider> registeredProviders)
        {
            _registeredProviders = registeredProviders;
        }

        public IEnumerable<GraphDescriptor> DecribeGraphs()
        {
            var descriptors = new List<GraphDescriptor>();

            foreach (var provider in _registeredProviders)
            {
                var descriptor = new GraphDescriptorImpl();
                provider.Describe(descriptor);
                descriptor.Freeze();
                descriptors.Add(descriptor);
            }

            return descriptors.AsEnumerable();
        }

        private class GraphDescriptorImpl : GraphDescriptor
        {
        }
    }
}