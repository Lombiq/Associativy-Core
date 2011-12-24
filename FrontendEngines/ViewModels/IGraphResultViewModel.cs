using System;
using Orchard;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphResultViewModel : ITransientDependency
    {
        dynamic Graph { get; set; }
        dynamic SearchForm { get; set; }
    }
}
