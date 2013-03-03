using Orchard.Environment.Extensions;

namespace Associativy.Models.Mind
{
    [OrchardFeature("Associativy")]
    public class MindSettings : IMindSettings
    {
        public string Algorithm { get; set; }
        public bool UseCache { get; set; }
        public int ZoomLevel { get; set; }
        public int ZoomLevelCount { get; set; }
        public int MaxDistance { get; set; }

        private static MindSettings _default = new MindSettings();
        public static MindSettings Default { get { return _default; } }


        public MindSettings()
        {
            Algorithm = "sophisticated";
            UseCache = false;
            ZoomLevel = 0;
            ZoomLevelCount = 10;
            MaxDistance = 3;
        }
    }
}