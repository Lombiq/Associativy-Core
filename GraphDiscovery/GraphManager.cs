using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;
using System.Diagnostics;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphManager : IGraphManager
    {
        private readonly IEnumerable<IGraphDescriptorProvider> _descriptorProviders;
        private readonly IDescriptorFilterer _descriptorFilterer;

        private IEnumerable<GraphDescriptor> _descriptors;
        private IEnumerable<GraphDescriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    _descriptors = Enumerable.Empty<GraphDescriptor>();
                    foreach (var provider in _descriptorProviders)
                    {
                        _descriptors = _descriptors.Union(provider.DecribeGraphs());
                    }
                }

                return _descriptors;
            }
        }
        

        public GraphManager(
            IEnumerable<IGraphDescriptorProvider> descriptorProviders,
            IDescriptorFilterer providerFilterer)
        {
            _descriptorProviders = descriptorProviders;
            _descriptorFilterer = providerFilterer;
        }


        public GraphDescriptor FindGraph(IGraphContext graphContext)
        {
            return FindGraphs(graphContext).LastOrDefault();
        }

        public IEnumerable<GraphDescriptor> FindGraphs(IGraphContext graphContext)
        {
            return _descriptorFilterer.FilterByMatchingGraphContext(Descriptors, graphContext);
        }

        public IEnumerable<GraphDescriptor> FindDistinctGraphs(IGraphContext graphContext)
        {
            var descriptors = new Dictionary<string, GraphDescriptor>();

            foreach (var descriptor in FindGraphs(graphContext))
            {
                if(!String.IsNullOrEmpty(descriptor.GraphName)) descriptors[descriptor.GraphName] = descriptor;
            }

            return descriptors.Values;
        }
    }
}