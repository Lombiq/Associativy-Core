using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.Services
{
    public interface IMemoryConnectionManager : IConnectionManager, IGraphStatisticsService, ITransientDependency
    {
        int GetConnectionCount();
    }
}
