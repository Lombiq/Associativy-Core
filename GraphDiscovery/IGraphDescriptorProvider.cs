using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// Surfaces graphs registered in the system
    /// </summary>
    /// <remarks>
    /// E.g. it gathers registered graph providers and describes them.
    /// </remarks>
    public interface IGraphDescriptorProvider : IDependency
    {
        IEnumerable<GraphDescriptor> DecribeGraphs();
    }
}
