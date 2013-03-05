using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Services;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    public interface IGraphDescriptor
    {
        string Name { get; }
        LocalizedString DisplayName { get; }
        IEnumerable<string> ContentTypes { get; }
        IGraphServices Services { get; }
    }
}
