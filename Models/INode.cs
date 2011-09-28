namespace Associativy.Models
{
    /// <summary>
    /// Used mainly to mark NodeParts
    /// 
    /// Very useful in INodeParams, as it makes it possible not to have to specify a TRecord too, if the constraint would be : NodePart
    /// </summary>
    public interface INode
    {
        string Label { get; set; }
    }
}
