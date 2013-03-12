using Associativy.Models;

namespace Associativy.Services
{
    /// <summary>
    /// Service for dealing with connections between nodes, persisting connections in the SQL database
    /// </summary>
    /// <typeparam name="TNodeToNodeConnectorRecord">Record type for node to node connectors</typeparam>
    public interface ISqlConnectionManager<TNodeToNodeConnectorRecord> : IConnectionManager
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
    }
}
