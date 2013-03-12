using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Services;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    public class DescribeContext
    {
        private readonly List<IGraphDescriptor> _descriptors;

        public IEnumerable<IGraphDescriptor> Descriptors { get { return _descriptors.AsEnumerable(); } }


        public DescribeContext()
        {
            _descriptors = new List<IGraphDescriptor>();
        }


        public virtual void DescribeGraph(string name, LocalizedString displayName, IEnumerable<string> contentTypes, Func<IGraphDescriptor, IGraphServices> graphServicesFactory)
        {
            if (String.IsNullOrEmpty(name) || displayName == null || String.IsNullOrEmpty(displayName.Text))
            {
                throw new ArgumentException("Associativy graphs should have their Name and DisplayName set properly.");
            }

            _descriptors.Add(new GraphDescriptor(name, displayName, contentTypes, graphServicesFactory));
        }
    }
}