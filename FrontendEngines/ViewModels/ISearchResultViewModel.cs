using System;
using Orchard;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface ISearchResultViewModel : ITransientDependency
    {
        dynamic Graph { get; set; }
        dynamic SearchForm { get; set; }
    }
}
