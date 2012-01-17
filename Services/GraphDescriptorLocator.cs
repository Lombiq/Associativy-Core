using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class GraphDescriptorLocator : IGraphDescriptorLocator
    {
        private readonly IEnumerable<IGraphDescriptor> _registeredGraphDescriptors;

        public GraphDescriptorLocator(IEnumerable<IGraphDescriptor> registeredGraphDescriptors)
        {
            _registeredGraphDescriptors = registeredGraphDescriptors;
        }

        public IGraphDescriptor FindGraphDescriptor(string technicalGraphName)
        {
            var graphDescriptor = (from c in _registeredGraphDescriptors
                                   where c.TechnicalGraphName == technicalGraphName
                                   select c).LastOrDefault(); // Last, so descriptors can be overridden

            return graphDescriptor;
        }

        public IEnumerable<IGraphDescriptor> FindGraphDescriptorsForContentType(string contentType)
        {
            // Might be worth storing graphDescriptors in a dictionary indexed by content types so it doesn't have to be recalculated.
            // But with a reasonable number of graphDescriptors it takes slightly less than nothing to run...
            var graphDescriptors = from c in _registeredGraphDescriptors
                                   where c.ContentTypes.Contains(contentType)
                                   select c;

            return graphDescriptors;
        }

        public IDictionary<string, IList<IGraphDescriptor>> FindGraphDescriptorsByRegisteredContentTypes()
        {
            var graphDescriptors = new Dictionary<string, IList<IGraphDescriptor>>();

            foreach (var graphDescriptor in _registeredGraphDescriptors)
            {
                foreach (var contentType in graphDescriptor.ContentTypes)
                {
                    if (!graphDescriptors.ContainsKey(contentType)) graphDescriptors[contentType] = new List<IGraphDescriptor>();
                    graphDescriptors[contentType].Add(graphDescriptor);
                }
            }

            return graphDescriptors;
        }
    }
}