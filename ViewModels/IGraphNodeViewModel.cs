
namespace Associativy.ViewModels
{
    public interface IGraphNodeViewModel
    {
        int Id { get; set; }
        string Label { get; set; }
        System.Collections.Generic.IList<int> NeighbourIds { get; set; }
    }

    public interface IGraphNodeViewModel<TNode> : IGraphNodeViewModel
     where TNode : Associativy.Models.INode
    {
        TNode Node { get; set; }
        void MapFromNode(TNode node);
    }
}
