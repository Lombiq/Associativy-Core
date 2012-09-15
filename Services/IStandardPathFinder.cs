using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations, by default using IConnectionManager
    /// </summary>
    /// <remarks>
    /// E.g. when using a graph database as storage, you can write your own IPathFinder and IConnectionManager implementation for optimized results.
    /// </remarks>
    public interface IStandardPathFinder : IPathFinder
    {
    }
}
