using System.Web.Routing;
using Orchard;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface ISearchViewModel : ITransientDependency
    {
        string Terms { get; set; }
        string[] TermsArray { get; }
        int ZoomLevel { get; set;  }
    }
}
