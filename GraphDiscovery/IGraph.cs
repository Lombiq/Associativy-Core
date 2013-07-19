using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    public interface IGraph
    {
        string Name { get; }
        LocalizedString DisplayName { get; }
        IEnumerable<string> ContentTypes { get; }
    }
}
