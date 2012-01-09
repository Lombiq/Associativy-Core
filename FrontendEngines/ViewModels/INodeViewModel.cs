using System;
using Orchard;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface INodeViewModel : ITransientDependency
    {
        IContent ContentItem { get; set; }
        void AddToZone(string zone, string content);
        void ClearZone(string zone);
        void SetZone(string zone, string content);
        string GetZoneContent(string zone);
    }
}
