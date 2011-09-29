using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;
using Orchard.Data;

namespace Associativy.Services
{
    public interface INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        #region Connection management
        bool AreConnected(int nodeId1, int nodeId2);
        void AddConnection(int nodeId1, int nodeId2);
        void AddConnection(TNodePart node1, TNodePart node2);
        IList<TNodeToNodeConnectorRecord> GetAllConnections();
        IList<int> GetNeighbourIds(int nodeId);
        int GetNeighbourCount(int nodeId);
        #endregion

        IList<string> GetSimilarTerms(string snippet, int maxCount = 10);
        
        #region Node CRUD
        IContentQuery<TNodePart, TNodePartRecord> ContentQuery { get; }
        TNodePart Create(TNodeParams nodeParams);
        TNodePart Get(int id);
        TNodePart Update(TNodeParams nodeParams);
        TNodePart Update(TNodePart node);
        void Delete(int id);
        #endregion
    }
}
