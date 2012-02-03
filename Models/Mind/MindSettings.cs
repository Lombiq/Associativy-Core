using Orchard.Environment.Extensions;
using System;
using Orchard.ContentManagement;

namespace Associativy.Models.Mind
{
    [OrchardFeature("Associativy")]
    public class MindSettings : IMindSettings
    {
        public MindAlgorithm Algorithm { get; set; }
        public bool UseCache { get; set; }
        public int ZoomLevel { get; set; }
        public int MaxZoomLevel { get; set; }
        public int MaxDistance { get; set; }
        public Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> QueryModifier { get; set; }

        public MindSettings()
        {
            Algorithm = MindAlgorithm.Sophisticated;
            UseCache = false;
            ZoomLevel = 0;
            MaxZoomLevel = 10;
            MaxDistance = 3;
            QueryModifier = (query) => query;
        }
    }
}