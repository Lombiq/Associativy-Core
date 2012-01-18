using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public class DescribeContextImpl : DescribeContext
    {
        private readonly Dictionary<string, DescribeFor> _describesByGraphName = new Dictionary<string, DescribeFor>();
        private readonly Dictionary<string, List<DescribeForImpl>> _describesByContentType = new Dictionary<string, List<DescribeForImpl>>();

        public override DescribeFor For(string graphName)
        {
            DescribeFor describeFor;

            if (!_describesByGraphName.TryGetValue(graphName, out describeFor))
            {
                describeFor = new DescribeForImpl(graphName);
                _describesByGraphName[graphName] = describeFor;
            }

            foreach (var contentType in descriptor.ContentTypes)
            {
                if (!_descriptorsByContentType.ContainsKey(contentType)) _descriptorsByContentType[contentType] = new List<IGraphDescriptor>();
                _descriptorsByContentType[contentType].Add(descriptor);
            }

            return describeFor;
        }

        public override GraphDescriptor Describe(IGraphContext graphContext)
        {
        }

        public override IEnumerable<GraphDescriptor> Describe(IContentContext contentContext)
        {
            throw new NotImplementedException();
        }
    }
}