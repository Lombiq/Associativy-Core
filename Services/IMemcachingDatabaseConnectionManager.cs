using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Models;

namespace Associativy.Services
{
    public interface IMemcachingDatabaseConnectionManager<TNodeToNodeConnectorRecord> : IDatabaseConnectionManager<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        void LoadConnections();
    }
}
