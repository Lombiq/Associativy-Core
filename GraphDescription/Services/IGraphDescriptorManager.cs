using Associativy.Models;
using Orchard;
using System.Collections.Generic;
using Associativy.GraphDescription.Contexts;

namespace Associativy.GraphDescription.Services
{
    /// <summary>
    /// Handles registered GraphDescriptors
    /// </summary>
    public interface IGraphDescriptorManager : IDependency
    {
        GraphDescriptor FindGraphDescriptor(IGraphContext graphContext);

        IEnumerable<GraphDescriptor> FindGraphDescriptors(IContentContext contentContext);
    }
}
