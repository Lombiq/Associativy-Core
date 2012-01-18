using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.GraphDescription
{
    public interface IGraphContext
    {
        /// <summary>
        /// Name of the graph used to identify it
        /// </summary>
        string GraphName { get; }
    }
}