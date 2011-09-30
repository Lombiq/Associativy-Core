using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;
using Orchard.Data;

namespace Associativy.Services
{
    public interface IConnectionManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        bool AreConnected(int nodeId1, int nodeId2);
        void Add(int nodeId1, int nodeId2);
        void Add(TNodePart node1, TNodePart node2);
        IList<TNodeToNodeConnectorRecord> GetAll();
        IList<int> GetNeighbourIds(int nodeId);
        int GetNeighbourCount(int nodeId);
    }
}
