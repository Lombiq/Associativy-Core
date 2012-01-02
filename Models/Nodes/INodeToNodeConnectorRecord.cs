namespace Associativy.Models
{
    public interface INodeToNodeConnectorRecord
    {
        int Id { get; set; }
        int Node1Id { get; set; }
        int Node2Id { get; set; }
    }
}
