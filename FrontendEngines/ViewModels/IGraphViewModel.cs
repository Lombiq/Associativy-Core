using System.Collections.Generic;
using Orchard;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphViewModel : ITransientDependency
    {
        int ZoomLevel { get; set; }
        Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}
