using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.GraphDiscovery;
using Associativy.Models.Services;

namespace Associativy.Services
{
    /// <summary>
    /// Provides information about a graph
    /// </summary>
    public interface IGraphStatisticsService
    {
        IGraphInfo GetGraphInfo();
        IEnumerable<int> GetBiggestNodes(int maxCount);
    }
}
