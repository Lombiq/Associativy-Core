using System;
using System.Web.Routing;

namespace Associativy.ViewModels
{
    public interface ISearchViewModel
    {
        string Terms { get; set; }
        string[] TermsArray { get; }
        RouteValueDictionary PostRouteValueDictionary { get; set; }
        string FetchUrl { get; set; }
    }
}
