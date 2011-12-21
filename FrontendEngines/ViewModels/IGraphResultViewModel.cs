using System.Collections.Generic;
using Orchard;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphResultViewModel : ITransientDependency
    {
        Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}
