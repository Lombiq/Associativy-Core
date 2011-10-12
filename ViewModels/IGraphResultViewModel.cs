using System;

namespace Associativy.ViewModels
{
    public interface IGraphResultViewModel
    {
        System.Collections.Generic.Dictionary<int, IGraphNodeViewModel> Nodes { get; set; }
    }
}
