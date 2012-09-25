using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.Models.Nodes
{
    public class NodeConnector : INodeToNodeConnector
    {
        public int Node1Id { get; set; }
        public int Node2Id { get; set; }
    }
}