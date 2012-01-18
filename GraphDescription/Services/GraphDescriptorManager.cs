using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;
using Associativy.GraphDescription.Contexts;

namespace Associativy.GraphDescription.Services
{
    [OrchardFeature("Associativy")]
    public class GraphDescriptorManager : IGraphDescriptorManager
    {
        private readonly IEnumerable<IGraphProvider> _registeredGraphDescriptors;

        public GraphDescriptorManager(IEnumerable<IGraphProvider> graphProviders)
        {
            _registeredGraphDescriptors = graphProviders;

            // This maybe not in ctor...
            var context = new DescribeContextImpl();
            foreach (var provider in graphProviders)
            {
                provider.Describe(context);
            }

            
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

        public GraphDescriptor FindGraphDescriptor(IGraphContext graphContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GraphDescriptor> FindGraphDescriptors(IContentContext contentContext)
        {
            throw new NotImplementedException();
        }
    }
}