using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeTitleLabelPart : AssociativyNodeLabelPart, ITitleAspect
    {
        public string Title { get { return Label; } }
    }
}