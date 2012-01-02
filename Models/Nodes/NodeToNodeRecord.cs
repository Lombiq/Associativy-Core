namespace Associativy.Models
{
    public abstract class NodeToNodeRecord : INodeToNodeConnectorRecord
    {
        public virtual int Id { get; set; }
        public virtual int Node1Id { get; set; }
        public virtual int Node2Id { get; set; }
    }
}