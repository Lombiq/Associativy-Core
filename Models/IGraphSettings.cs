using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Associativy.Models
{
    public interface IGraphSettings : ITransientDependency
    {
        int ZoomLevel { get; set; }
    }
}
