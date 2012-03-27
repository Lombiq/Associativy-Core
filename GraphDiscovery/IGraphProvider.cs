using Associativy.Services;
using Orchard;
using Orchard.Localization;
using Associativy.GraphDiscovery;
using System.Collections.Generic;
using Orchard.Events;

namespace Associativy.GraphDiscovery
{
    public interface IGraphProvider : IDependency
    {
        void Describe(DescribeContext describeContext);
    }
}
