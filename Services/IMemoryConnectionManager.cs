using Orchard;

namespace Associativy.Services
{
    public interface IMemoryConnectionManager : IConnectionManager, ITransientDependency
    {
        int GetConnectionCount();
    }
}
