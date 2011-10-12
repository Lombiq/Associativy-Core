using System.Collections.Generic;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphNodeViewModel<TNode> : Associativy.ViewModels.IGraphNodeViewModel<TNode>
         where TNode : INode
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public IList<int> NeighbourIds { get; set; }
        public TNode Node { get; set; }

        public GraphNodeViewModel()
        {
            NeighbourIds = new List<int>();
        }

        public virtual void MapFromNode(TNode node)
        {
            Id = node.Id;
            Label = node.Label;
        }
    }
}