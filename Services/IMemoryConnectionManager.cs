using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.Services
{
    public interface IMemoryConnectionManager : IConnectionManager, IDependency
    {
        int GetConnectionCount();
    }
}
