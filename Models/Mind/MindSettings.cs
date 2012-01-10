using Orchard.Environment.Extensions;

namespace Associativy.Models.Mind
{
    [OrchardFeature("Associativy")]
    public class MindSettings : IMindSettings
    {
        public MindAlgorithms Algorithm { get; set; }
        public bool UseCache { get; set; }
        public int ZoomLevel { get; set; }
        public int MaxZoomLevel { get; set; }
        public int MaxDistance { get; set; }

        public MindSettings()
        {
            Algorithm = MindAlgorithms.Sophisticated;
            UseCache = false;
            ZoomLevel = 0;
            MaxZoomLevel = 10;
            MaxDistance = 3;
        }
    }
}