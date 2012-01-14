using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPartRecord : ContentPartRecord
    {
        public virtual string Label { get; set; }
        public virtual string InvariantLabel { get; set; }
    }
}