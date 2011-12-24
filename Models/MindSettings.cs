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
        public int ZoomLevel { get; set; }
        public int MaxDistance { get; set; }

        public MindSettings()
        {
            Algorithm = MindAlgorithms.Sophisticated;
            UseCache = true;
            ZoomLevel = 0;
            MaxDistance = 3;
        }
    }
}