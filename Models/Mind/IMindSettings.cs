using Orchard;

namespace Associativy.Models.Mind
{
    public enum MindAlgorithms
    {
        Simple,
        Sophisticated
    }

    public interface IMindSettings
    {
        MindAlgorithms Algorithm { get; set; }
        bool UseCache { get; set; }
        int ZoomLevel { get; set; }

        /// <summary>
        /// Upper bound of the graph zoom levels (lower bound is always zero).
        /// </summary>
        int MaxZoomLevel { get; set; }

        int MaxDistance { get; set; }
    }
}
