using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;

namespace Associativy.Models
{
    public interface IAssociativyNodeLabelAspect : IContent
    {
        string Label { get; set; }
    }
}
