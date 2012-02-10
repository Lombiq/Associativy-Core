using Associativy.Services;
using Orchard;
using Orchard.Localization;
using Associativy.GraphDiscovery;
using System.Collections.Generic;

namespace Associativy.GraphDiscovery
{
    public interface IGraphProvider : IDependency
    {
        void Describe(GraphDescriptor graphDescriptor);
    }
}
