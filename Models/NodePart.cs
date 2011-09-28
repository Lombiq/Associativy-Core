using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    abstract public class NodePart<TRecord> : ContentPart<TRecord> where TRecord : NodePartRecord
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