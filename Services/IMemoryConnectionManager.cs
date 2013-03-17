using Orchard;

namespace Associativy.Services
{
    public interface IMemoryConnectionManager : IConnectionManager, IGraphAwareService, ITransientDependency
    {
        int GetConnectionCount();
    }
}
