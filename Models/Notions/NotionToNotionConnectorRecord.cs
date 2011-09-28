using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionToNotionConnectorRecord : INodeToNodeConnectorRecord
    {
        public virtual int Id { get; set; }
        public virtual int Record1Id { get; set; }
        public virtual int Record2Id { get; set; }
    }
}