using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodePart : ContentPart
    {
        public IAssociativyContext CurrentContext { get; set; }
    }
}