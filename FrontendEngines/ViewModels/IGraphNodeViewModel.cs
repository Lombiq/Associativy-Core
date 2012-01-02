using System.Collections.Generic;
using Orchard;
using Associativy.Models;

namespace Associativy.FrontendEngines.ViewModels
{
    public interface IGraphNodeViewModel : ITransientDependency
    {
        // Setek itt nem kellenek
        int Id { get; set; }
        string Label { get; set; }
        IList<INode> Neighbours { get; set; }
    }

    public interface IGraphNodeViewModel<TNode> : IGraphNodeViewModel
     where TNode : INode
    {
        TNode Node { get; set; }
        void MapFromNode(TNode node);
    }
}
