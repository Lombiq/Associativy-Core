
namespace Associativy.Services
{
    /// <summary>
    /// Contains services that deal with node to node connections and paths
    /// </summary>
    public interface IPathServices
    {
        IConnectionManager ConnectionManager { get; }
        IPathFinder PathFinder { get; }
    }
}
