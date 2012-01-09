using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Associativy.FrontendEngines.NodeFilters
{
    [OrchardFeature("Associativy")]
    public class TitleFilter : NodeFilter
    {
        public override INodeViewModel Apply(IContent node, INodeViewModel viewModel, string frontendEngine)
        {
            if (node.Has<ITitleAspect>())
            {
                viewModel.AddToZone("Header", node.As<ITitleAspect>().Title);
            }

            return viewModel;
        }
    }
}