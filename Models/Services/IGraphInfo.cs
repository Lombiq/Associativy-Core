using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Associativy.Models.Services
{
    public interface IGraphInfo
    {
        int NodeCount { get; }
        int ConnectionCount { get; }
        int CentralNodeId { get; }
    }
}
