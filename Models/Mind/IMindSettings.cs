using Orchard;

namespace Associativy.Models.Mind
{
    public enum MindAlgorithms
    {
        Simple,
        Sophisticated
    }

    public interface IMindSettings : ITransientDependency
    {
        MindAlgorithms Algorithm { get; set; }
        bool UseCache { get; set; }
        int ZoomLevel { get; set; }
        int MaxDistance { get; set; }
    }
}
