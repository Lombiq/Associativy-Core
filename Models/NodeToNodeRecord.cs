using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class NodeToNodeRecord
    {
        public virtual int Id { get; set; }
        public virtual int NodeRecord1Id { get; set; }
        public virtual int NodeRecord2Id { get; set; }
    }
}