using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Models;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes, persisting connections in the database
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public interface IDatabaseConnectionManager<TNodeToNodeConnectorRecord> : IConnectionManager
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
    }
}
