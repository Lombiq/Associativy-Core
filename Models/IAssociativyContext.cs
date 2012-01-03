using Orchard;
using Orchard.Localization;

namespace Associativy.Models
{
    public interface IAssociativyContext : IDependency
    {
        LocalizedString Name { get; }
        string TechnicalName { get; }
        string[] ContentTypeNames { get; }
    }
}
