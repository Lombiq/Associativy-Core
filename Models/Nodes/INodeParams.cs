namespace Associativy.Models
{
    public interface INodeParams<TNodePart> : INode where TNodePart : INode
    {
        string ContentTypeName { get; }
        TNodePart MapToNode(TNodePart part);
    }
}
