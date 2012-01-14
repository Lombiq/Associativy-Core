using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyGraphDescriptorLocator : IAssociativyGraphDescriptorLocator
    {
        private readonly IEnumerable<IAssociativyGraphDescriptor> _registeredGraphDescriptors;

        public AssociativyGraphDescriptorLocator(IEnumerable<IAssociativyGraphDescriptor> registeredGraphDescriptors)
        {
            _registeredGraphDescriptors = registeredGraphDescriptors;
        }

        public IAssociativyGraphDescriptor FindGraphDescriptor(string technicalGraphName)
        {
            var graphDescriptor = (from c in _registeredGraphDescriptors
                                   where c.TechnicalGraphName == technicalGraphName
                                   select c).FirstOrDefault();

            return graphDescriptor;
        }

        public IEnumerable<IAssociativyGraphDescriptor> FindGraphDescriptorsForContentType(string contentType)
        {
            // Might be worth storing graphDescriptors in a dictionary indexed by content types so it doesn't have to be recalculated.
            // But with a reasonable number of graphDescriptors it takes slightly less than nothing to run...
            var graphDescriptors = from c in _registeredGraphDescriptors
                                   where c.ContentTypes.Contains(contentType)
                                   select c;

            return graphDescriptors;
        }

        public IDictionary<string, IList<IAssociativyGraphDescriptor>> FindGraphDescriptorsByRegisteredContentTypes()
        {
            var associativyGraphDescriptors = new Dictionary<string, IList<IAssociativyGraphDescriptor>>();

            foreach (var graphDescriptor in _registeredGraphDescriptors)
            {
                foreach (var contentType in graphDescriptor.ContentTypes)
                {
                    if (!associativyGraphDescriptors.ContainsKey(contentType)) associativyGraphDescriptors[contentType] = new List<IAssociativyGraphDescriptor>();
                    associativyGraphDescriptors[contentType].Add(graphDescriptor);
                }
            }

            return associativyGraphDescriptors;
        }
    }
}