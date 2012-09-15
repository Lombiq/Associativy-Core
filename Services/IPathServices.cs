using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Containes services that deal with node to node connections and paths
    /// </summary>
    public interface IPathServices
    {
        IConnectionManager ConnectionManager { get; }
        IPathFinder PathFinder { get; }
    }
}
