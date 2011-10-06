using System;
using System.Collections.Generic;
using Associativy.Models;

namespace Associativy.ViewModels
{
    public interface IGraphNodeViewModel: INode
    {
        IList<int> NeighbourIds { get; set; }
    }
}
