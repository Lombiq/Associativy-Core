using Associativy.Services;
using Orchard;
using Orchard.Localization;

namespace Associativy.Models
{
    // Maybe also set the search form's content type from here?
    public interface IAssociativyContext : IDependency
    {
        LocalizedString GraphName { get; }
        string TechnicalGraphName { get; }
        string[] ContentTypes { get; }
        int MaxZoomLevel { get; }
        IConnectionManager ConnectionManager { get; }
    }
}
