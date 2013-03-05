using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.Models.Nodes
{
    [OrchardFeature("Associativy")]
    public class NodeConnector : INodeToNodeConnector
    {
        public int Node1Id { get; set; }
        public int Node2Id { get; set; }
    }
}