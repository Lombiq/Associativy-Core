using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDiscovery
{
    public class GraphDescriptorFilterer : IGraphDescriptorFilterer
    {
        public IEnumerable<IGraphDescriptor> FilterByMatchingGraphContext(IEnumerable<IGraphDescriptor> descriptors, IGraphContext graphContext) 
        {
            var filteredDescriptors = descriptors;

            if (!String.IsNullOrEmpty(graphContext.Name))
            {
                // Catch-alls with empty name property are kept
                filteredDescriptors = filteredDescriptors.Where((descriptor) => descriptor.Name == graphContext.Name || String.IsNullOrEmpty(descriptor.Name));
            }


            if (graphContext.ContentTypes != null && graphContext.ContentTypes.Count() != 0)
            {
                if (filteredDescriptors.Count() == 0)
                {
                    // If there are no descriptors with the graph context, try only with content types
                    filteredDescriptors = descriptors;
                }

                filteredDescriptors = filteredDescriptors.Where((descriptor) =>
                {

                    foreach (var contentType in graphContext.ContentTypes)
                    {
                        // A descriptor is only suitable if it has no content types specified (catch-all) or contains all content 
                        // types listed in the context.
                        if (descriptor.ContentTypes == null || descriptor.ContentTypes.Count() == 0) return true;
                        if (!descriptor.ContentTypes.Contains(contentType)) return false;
                    }

                    return true;
                });
            }

            // Catch-alls will be at the top, so the more specific descriptors at the bottom.
            // Also, because OrderBy is a stable sort, registration (dependency) order is kept.
            filteredDescriptors = filteredDescriptors.OrderBy(descriptor => descriptor.ContentTypes != null && descriptor.ContentTypes.Count() != 0);


            return filteredDescriptors;
        }
    }
}