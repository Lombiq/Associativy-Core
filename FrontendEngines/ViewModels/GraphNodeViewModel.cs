﻿using System.Collections.Generic;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public class GraphNodeViewModel<TNode> : IGraphNodeViewModel<TNode>
         where TNode : INode
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public IList<INode> Neighbours { get; set; }
        public TNode Node { get; set; }

        public GraphNodeViewModel()
        {
            Neighbours = new List<INode>();
        }

        public virtual void MapFromNode(TNode node)
        {
            Id = node.Id;
            Label = node.Label;
        }
    }
}