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
        private readonly Dictionary<string, DescribeFor> _describes = new Dictionary<string, DescribeFor>();

        public override DescribeFor For(string graphName, LocalizedString displayName)
        {
            DescribeFor describeFor;
            if (!_describes.TryGetValue(graphName, out describeFor))
            {
                describeFor = new DescribeForImpl(target, name, description);
                _describes[graphName] = describeFor;
            }
            return describeFor;
        }
    }
}