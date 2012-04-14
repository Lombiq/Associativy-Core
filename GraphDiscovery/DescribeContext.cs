using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class DescribeContext
    {
        private readonly List<GraphDescriptor> _descriptors;

        public IEnumerable<GraphDescriptor> Descriptors
        {
            get
            {
                return _descriptors.AsEnumerable();
            }
        }

        public DescribeContext()
        {
            _descriptors = new List<GraphDescriptor>();
        }

        public void DescribeGraph(string name, LocalizedString displayName, IEnumerable<string> contentTypes, IConnectionManager connectionManager)
        {
            if (String.IsNullOrEmpty(name) || displayName == null || String.IsNullOrEmpty(displayName.Text))
            {
                throw new ArgumentException("Associativy graphs should have their Name and DisplayName set properly");
            }

            _descriptors.Add(new GraphDescriptor(name, displayName, contentTypes, connectionManager));
        }
    }
}