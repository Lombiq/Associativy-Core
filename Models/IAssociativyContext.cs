using System;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard;

namespace Associativy.Models
{
    public interface IAssociativyContext : IDependency
    {
        LocalizedString Name { get; }
        string TechnicalName { get; }
        string[] ContentTypeNames { get; }
    }
}
