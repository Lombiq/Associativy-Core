using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.FrontendEngines.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines.NodeFilters
{
    [OrchardFeature("Associativy")]
    public abstract class NodeFilter : INodeFilter
    {
        public virtual float Weight
        {
            get { return 0; }
        }

        public abstract INodeViewModel Apply(IContent node, INodeViewModel viewModel, string frontendEngine);

        public int CompareTo(INodeFilter other)
        {
            return Weight.CompareTo(other.Weight);
        }
    }
}