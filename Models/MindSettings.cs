using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.Models
{
    public class MindSettings : Associativy.Models.IMindSettings
    {
        public MindAlgorithms Algorithm { get; set; }
        public bool UseCache { get; set; }

        public MindSettings()
        {
            Algorithm = MindAlgorithms.Sophisticated;
            UseCache = true;
        }
    }
}