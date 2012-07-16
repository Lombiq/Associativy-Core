using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.GraphDiscovery;
using Orchard;

namespace Associativy.Services
{
    public interface IMemoryConnectionManager : IConnectionManager, IDependency
    {
        int GetConnectionCount(IGraphContext graphContext);
        void Connect(IGraphContext graphContext, int connectionId, int node1Id, int node2Id);
    }
}
