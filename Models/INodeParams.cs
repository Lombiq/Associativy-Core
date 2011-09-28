using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Associativy.Models
{
    public interface INodeParams<TNodePart> : INode where TNodePart : INode
    {
        string ContentTypeName { get; }
        TNodePart MapToPart(TNodePart part);
    }
}
