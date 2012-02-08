﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPart : ContentPart<AssociativyNodeLabelPartRecord>
    {
        public string Label
        {
            get { return Record.Label; }
            set
            {
                if (String.IsNullOrEmpty(value)) return;
                Record.Label = value;
                Record.UpperInvariantLabel = value.ToUpperInvariant();
            }
        }
    }
}