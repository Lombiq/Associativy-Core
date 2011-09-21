using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.Models
{
    public class GraphNode<T> where T : NodePartRecord
    {
        public T Node { get; set; }
        public int MinimumDepth { get; set; }
        public bool IsDeadEnd { get; set; }

        public GraphNode(T node)
        {
            Node = node;
            MinimumDepth = int.MaxValue;
            IsDeadEnd = false;
        }
    }
}