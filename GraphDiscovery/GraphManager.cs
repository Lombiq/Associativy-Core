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
        private GraphDescribeContextImpl _describeContext;

        public GraphManager(IEnumerable<IGraphProvider> registeredProviders)
        {
            _registeredProviders = registeredProviders;
            CompileProviders(); // Maybe should be called elsewhere, lazily, but is freaking fast (15 ticks).
        }

        private void CompileProviders()
        {
            _describeContext = new GraphDescribeContextImpl();

            foreach (var provider in _registeredProviders)
            {
                provider.Describe(_describeContext);
            }
        }

        public GraphDescriptor FindLastDescriptor(IGraphContext graphContext)
        {
            return FindDescriptors(graphContext).Last();
        }

        public IEnumerable<GraphDescriptor> FindDescriptors(IGraphContext graphContext)
        {
            // That's very fast (few dozen ticks), so there's no point in caching anything.
            // If it gets heavy, could be stored in an instance cache by context.
            IEnumerable<GraphDescriptorImpl> descriptors = _describeContext.GraphDescriptors;

            if (!String.IsNullOrEmpty(graphContext.GraphName))
            {
                descriptors = descriptors.Where((descriptor) => descriptor.GraphName == graphContext.GraphName);
            }

            if (graphContext.ContentTypes != null && graphContext.ContentTypes.Count() != 0)
            {
                descriptors = descriptors.Where((descriptor) =>
                    {
                        // A desciptor is only suitable if it contains all content types listed in the context
                        foreach (var contentType in graphContext.ContentTypes)
                        {
                            if (!descriptor.ContentTypes.Contains(contentType)) return false;
                        }

                        return true;
                    });
            }

            return descriptors;
        }


        private class GraphDescribeContextImpl : GraphDescribeContext
        {
            private readonly List<GraphDescriptorImpl> _graphDescriptors = new List<GraphDescriptorImpl>();
            public List<GraphDescriptorImpl> GraphDescriptors
            {
                get { return _graphDescriptors; }
            }
            
            public override void As(string graphName, LocalizedString displayGraphName, IEnumerable<string> contentTypes, IConnectionManager connectionManager)
            {
                _graphDescriptors.Add(new GraphDescriptorImpl(graphName, displayGraphName, contentTypes, connectionManager));
            }
        }

        private class GraphDescriptorImpl : GraphDescriptor
        {
            public GraphDescriptorImpl(string graphName, LocalizedString displayGraphName, IEnumerable<string> contentTypes, IConnectionManager connectionManager)
            {
                GraphName = graphName;
                DisplayGraphName = displayGraphName;
                ContentTypes = contentTypes;
                ConnectionManager = connectionManager;
            }
        }
    }
}