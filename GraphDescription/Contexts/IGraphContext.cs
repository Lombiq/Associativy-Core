using System;

namespace Associativy.GraphDescription.Contexts
{
    public interface IGraphContext
    {
        /// <summary>
        /// Name of the graph used to identify it
        /// </summary>
        string GraphName { get; }
    }
}
