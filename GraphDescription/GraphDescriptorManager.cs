using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription
{
    [OrchardFeature("Associativy")]
    public class GraphDescriptorManager : IGraphDescriptorManager
    {
        private readonly IEnumerable<IGraphDescriptor> _registeredDescriptors;
        private readonly Dictionary<string, IGraphDescriptor> _descriptorsByGraphName = new Dictionary<string, IGraphDescriptor>();
        private readonly Dictionary<string, List<IGraphDescriptor>> _descriptorsByContentType = new Dictionary<string, List<IGraphDescriptor>>();

        public GraphDescriptorManager(IEnumerable<IGraphDescriptor> registeredDescriptors)
        {
            _registeredDescriptors = registeredDescriptors;
            CompileDescriptors(); // Maybe should be called elsewhere...
        }

        private void CompileDescriptors()
        {
            foreach (var descriptor in _registeredDescriptors)
            {
                _descriptorsByGraphName[descriptor.GraphName] = descriptor; // Last one wins

                foreach (var contentType in descriptor.ContentTypes)
                {
                    if (!_descriptorsByContentType.ContainsKey(contentType)) _descriptorsByContentType[contentType] = new List<IGraphDescriptor>();
                    _descriptorsByContentType[contentType].Add(descriptor);
                }
            }
        }

        public IGraphDescriptor FindDescriptor(IGraphContext graphContext)
        {
            IGraphDescriptor graphDescriptor;

            if (!_descriptorsByGraphName.TryGetValue(graphContext.GraphName, out graphDescriptor))
            {
                throw new ApplicationException("No graph descriptor found for the graph " + graphContext.GraphName + ".");
            }

            return graphDescriptor;
        }

        public bool TryFindDescriptorsForContentType(IContentContext contentContext, out IEnumerable<IGraphDescriptor> graphDescriptors)
        {
            List<IGraphDescriptor> graphDescriptorList;

            var wasFound = _descriptorsByContentType.TryGetValue(contentContext.ContentType, out graphDescriptorList);
            graphDescriptors = graphDescriptorList;

            return wasFound;
        }
    }
}