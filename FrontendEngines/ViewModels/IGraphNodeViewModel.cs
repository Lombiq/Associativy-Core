using Orchard;
using System.Collections.Generic;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    // TODO: is non-generic needed?
    public interface IGraphNodeViewModel : ITransientDependency
    {
        int Id { get; set; }
        string Label { get; set; }
        IList<int> NeighbourIds { get; set; }
        //void MapFromNode(INode node);
    }

    public interface IGraphNodeViewModel<TNode> : IGraphNodeViewModel
     where TNode : Associativy.Models.INode
    {
        TNode Node { get; set; }
        void MapFromNode(TNode node);
    }
}
