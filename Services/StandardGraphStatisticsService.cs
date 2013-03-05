using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    public interface IStandardGraphStatisticsService : IGraphStatisticsService, IDependency
    {
    }

    [OrchardFeature("Associativy")]
    public class StandardGraphStatisticsService : IStandardGraphStatisticsService
    {
        public Models.Services.IGraphInfo GetGraphInfo()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetBiggestNodes(int maxCount)
        {
            throw new NotImplementedException();
        }
    }
}
