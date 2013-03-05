namespace Associativy.Models
{
    public interface INodeToNodeConnector
    {
        int Node1Id { get; set; }
        int Node2Id { get; set; }
    }
}
