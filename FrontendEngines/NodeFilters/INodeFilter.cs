using System;
using Orchard;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines.NodeFilters
{
    public interface INodeFilter : IComparable<INodeFilter>, IDependency
    {
        float Weight { get; }
        INodeViewModel Apply(IContent node, INodeViewModel viewModel, string frontendEngine);
    }
}
