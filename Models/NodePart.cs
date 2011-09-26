using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Associativy.Models
{
    //public abstract class NodePart<T> : ContentPart<T> where T : NodePartRecord
    [OrchardFeature("Associativy")]
    public class NodePart : ContentPart<NodePartRecord>, INode
    {
        [Required]
        public string Label
        {
            get { return Record.Label; }
            set { Record.Label = value; }
        }

        // LazyField neighbours?
    }
}