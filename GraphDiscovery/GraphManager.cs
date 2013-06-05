using System;
using System.Collections.Generic;
using System.Linq;

namespace Associativy.GraphDiscovery
{
    public class GraphManager : IGraphManager
    {
        private readonly IEnumerable<IGraphProvider> _graphProviders;
        private readonly IGraphDescriptorFilterer _descriptorFilterer;

        private IEnumerable<IGraphDescriptor> _descriptors;
        private IEnumerable<IGraphDescriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    _descriptors = Enumerable.Empty<IGraphDescriptor>();
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


        public IGraphDescriptor FindGraph(IGraphContext graphContext)
        {
            return FindGraphs(graphContext).LastOrDefault();
        }

        public IEnumerable<IGraphDescriptor> FindGraphs(IGraphContext graphContext)
        {
            var descriptors = new Dictionary<string, IGraphDescriptor>();

            foreach (var descriptor in _descriptorFilterer.FilterByMatchingGraphContext(Descriptors, graphContext))
            {
                if(!String.IsNullOrEmpty(descriptor.Name)) descriptors[descriptor.Name] = descriptor;
            }

            return descriptors.Values;
        }
    }
}