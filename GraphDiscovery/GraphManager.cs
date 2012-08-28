using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public class GraphManager : IGraphManager
    {
        private readonly IEnumerable<IGraphProvider> _graphProviders;
        private readonly IGraphDescriptorFilterer _descriptorFilterer;

        private IEnumerable<GraphDescriptor> _descriptors;
        private IEnumerable<GraphDescriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    _descriptors = Enumerable.Empty<GraphDescriptor>();
                    var describeContext = new DescribeContext();
                    foreach (var provider in _graphProviders)
                    {
                        provider.Describe(describeContext);
                    }
                    _descriptors = describeContext.Descriptors;
                }

                return _descriptors;
            }
        }
        

        public GraphManager(
            IEnumerable<IGraphProvider> graphProviders,
            IGraphDescriptorFilterer descriptorFilterer)
        {
            _graphProviders = graphProviders;
            _descriptorFilterer = descriptorFilterer;
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