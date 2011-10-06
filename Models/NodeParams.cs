using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    /// <summary>
    /// Abstract implementation of INodeParams for further use
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class NodeParams<TNode> : INodeParams<TNode> where TNode : INode
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public abstract string ContentTypeName { get; }

        public virtual TNode MapToNode(TNode node)
        {
            node.Label = Label;

            return node;
        }
    }
}