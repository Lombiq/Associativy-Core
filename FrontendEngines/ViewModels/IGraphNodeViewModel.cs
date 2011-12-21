using System.Collections.Generic;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphNodeViewModel : ITransientDependency
    {
        int Id { get; set; }
        string Label { get; set; }
        IList<int> NeighbourIds { get; set; }
    }

    public interface IGraphNodeViewModel<TNode> : IGraphNodeViewModel
     where TNode : INode
    {
        TNode Node { get; set; }
        void MapFromNode(TNode node);
    }
}
