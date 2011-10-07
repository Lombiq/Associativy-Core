using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;

namespace Associativy.Services
{
    public interface IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        bool AreNeighbours(int nodeId1, int nodeId2);
        void Add(int nodeId1, int nodeId2);
        void Add(TNodePart node1, TNodePart node2);
        void DeleteMany(int nodeId);
        void Delete(int id);
        IList<TNodeToNodeConnectorRecord> GetAll();
        IList<int> GetNeighbourIds(int nodeId);
        int GetNeighbourCount(int nodeId);
    }
}
