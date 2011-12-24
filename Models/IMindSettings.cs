using System;
using Orchard;

namespace Associativy.Models
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
    }
}
