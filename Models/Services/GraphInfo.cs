
namespace Associativy.Models.Services
{
    public class GraphInfo : IGraphInfo
    {
        public int NodeCount { get; set; }
        public int ConnectionCount { get; set; }
        public int CentralNodeId { get; set; }
    }
}