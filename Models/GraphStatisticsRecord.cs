using System.ComponentModel.DataAnnotations;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy.GraphStatisticsServices")]
    public class GraphStatisticsRecord
    {
        public virtual int Id { get; set; }
        [StringLength(1024)]
        public virtual string GraphName { get; set; }
        public virtual int NodeCount { get; set; }
        public virtual int ConnectionCount { get; set; }
        public virtual int CentralNodeId { get; set; }


        public GraphStatisticsRecord()
        {
            NodeCount = 0;
            ConnectionCount = 0;
        }
    }
}