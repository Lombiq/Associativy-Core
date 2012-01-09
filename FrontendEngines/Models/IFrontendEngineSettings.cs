using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard;

namespace Associativy.FrontendEngines.Models
{
    public interface IFrontendEngineSettings : IDependency
    {
        Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> GraphQueryModifier { get; }
    }
}
