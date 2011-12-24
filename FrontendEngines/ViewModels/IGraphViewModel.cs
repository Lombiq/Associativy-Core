using System.Collections.Generic;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphViewModel : ITransientDependency
    {
        Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}
