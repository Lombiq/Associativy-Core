using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    public abstract class NodeToNodeConnectorRecord : INodeToNodeConnector
    {
        public virtual int Id { get; set; }
        public virtual int Node1Id { get; set; }
        public virtual int Node2Id { get; set; }
    }
}