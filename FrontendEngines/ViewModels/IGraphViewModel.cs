using System.Collections.Generic;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphViewModel : ITransientDependency
    {
        IGraphSettings Settings { get; set; }
        Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}
