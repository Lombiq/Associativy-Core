using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;
using Orchard.Data;

namespace Associativy.Services
{
    public interface INodeManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        IRepository<TNodeToNodeConnectorRecord> NodeToNodeRecordRepository { get; }
        IRepository<TNodePartRecord> NodePartRecordRepository { get; }
        void AddConnection(int nodeId1, int nodeId2);
        void AddConnection(TNodePart node1, TNodePart node2);
        bool AreConnected(int nodeId1, int nodeId2);
        TNodePart CreateNode<TNodeParams>(TNodeParams nodeParams) where TNodeParams : Associativy.Models.INodeParams<TNodePart>;
        void DeleteNode(int id);
        int GetNeighbourCount(int nodeId);
        System.Collections.Generic.IList<int> GetNeighbourIds(int nodeId);
        TNodePart GetNode(int id);
        List<string> GetSimilarTerms(string snippet, int maxCount = 10);
        IEnumerable<TNodePart> GetSucceededNodes(List<List<int>> succeededPaths);
    }
}
