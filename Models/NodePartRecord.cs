using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class NodePartRecord : ContentPartRecord
    {
        public virtual string Label { get; set; }
    }
}