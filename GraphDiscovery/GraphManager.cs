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
        private readonly IEnumerable<IGraphProvider> _registeredProviders;
        private readonly IDescriptorFilterer _providerFilterer;

        private readonly List<GraphDescriptor> _descriptors;
        private List<GraphDescriptor> Descriptors
        {
            get
            {
                if (_descriptors.Count == 0) DescribeGraphs();
                return _descriptors;
            }
        }
        

        public GraphManager(
            IEnumerable<IGraphProvider> registeredProviders,
            IDescriptorFilterer providerFilterer)
        {
            _registeredProviders = registeredProviders;
            _providerFilterer = providerFilterer;
            _descriptors = new List<GraphDescriptor>();
        }


        public GraphDescriptor FindGraph(IGraphContext graphContext)
        {
            return FindGraphs(graphContext).LastOrDefault();
        }

        public IEnumerable<GraphDescriptor> FindGraphs(IGraphContext graphContext)
        {
            // That's very fast (~70 ticks), so there's no point in caching anything.
            // If it gets heavy, could be stored in an instance cache by context.
            return _providerFilterer.FilterByMatchingGraphContext(Descriptors, graphContext);
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

        private void DescribeGraphs()
        {
            foreach (var provider in _registeredProviders)
            {
                var descriptor = new GraphDescriptorImpl();
                provider.Describe(descriptor);
                descriptor.Freeze();
                _descriptors.Add(descriptor);
            }
        }

        private class GraphDescriptorImpl : GraphDescriptor
        {
        }
    }
}