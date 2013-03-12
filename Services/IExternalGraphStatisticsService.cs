using Associativy.Models.Services;
using Orchard;

namespace Associativy.Services
{
    /// <summary>
    /// Service for keeping track of statistical graph data independent from graph services
    /// </summary>
    public interface IExternalGraphStatisticsService : IGraphAwareService, ITransientDependency
    {
        IGraphInfo GetGraphInfo();
        void SetNodeCount(int count);
        void SetConnectionCount(int count);
        void SetCentralNodeId(int id);
    }


    public static class ExternalGraphStatisticsServiceExtensions
    {
        public static void AdjustNodeCount(this IExternalGraphStatisticsService service, int difference)
        {
            service.SetNodeCount(service.GetGraphInfo().NodeCount + difference);
        }

        public static void AdjustConnectionCount(this IExternalGraphStatisticsService service, int difference)
        {
            service.SetConnectionCount(service.GetGraphInfo().ConnectionCount + difference);
        }
    }
}