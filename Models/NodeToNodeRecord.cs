﻿namespace Associativy.Models
{
    public abstract class NodeToNodeRecord : INodeToNodeConnectorRecord
    {
        public virtual long Id { get; set; }
        public virtual int Record1Id { get; set; }
        public virtual int Record2Id { get; set; }
    }
}