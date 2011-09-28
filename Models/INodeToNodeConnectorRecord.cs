namespace Associativy.Models
{
    public interface INodeToNodeConnectorRecord
    {
        long Id { get; set; }
        int Record1Id { get; set; }
        int Record2Id { get; set; }
    }
}
