using Orchard;
using Orchard.Localization;

namespace Associativy.Models
{
    public interface IAssociativyContext : IDependency
    {
        LocalizedString GraphName { get; }
        string TechnicalGraphName { get; }
        string[] ContentTypeNames { get; }
        int MaxZoomLevel { get; }
    }
}
