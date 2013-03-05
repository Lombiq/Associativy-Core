using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Associativy.Models.Services
{
    public interface IPathFinderSettings
    {
        int MaxDistance { get; set; }
        bool UseCache { get; set; }
    }
}
