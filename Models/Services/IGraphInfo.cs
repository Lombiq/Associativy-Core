
namespace Associativy.Models.Services
{
    public interface IGraphInfo
    {
        int NodeCount { get; }
        int ConnectionCount { get; }
        int CentralNodeId { get; }
    }
}
