using System;
namespace Associativy.Models
{
    public interface INodeToNodeConnectorRecord
    {
        int Id { get; set; }
        int Record1Id { get; set; }
        int Record2Id { get; set; }
    }
}
