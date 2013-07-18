using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Associativy.Models.Services;
using Orchard.Data;

namespace Associativy.Services
{
    public abstract class SqlConnectionManagerBase<TNodeToNodeConnectorRecord> : GraphAwareServiceBase, ISqlConnectionManager<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        protected readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        protected readonly IMemoryConnectionManager _memoryConnectionManager;
        protected readonly IGraphEventHandler _graphEventHandler;


        protected SqlConnectionManagerBase(
            IGraphDescriptor graphDescriptor,
            IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository,
            Func<IGraphDescriptor, IMemoryConnectionManager> memoryConnectionManagerFactory,
            IGraphEventHandler graphEventHandler)
            : base(graphDescriptor)
        {
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _memoryConnectionManager = memoryConnectionManagerFactory(graphDescriptor);
            _graphEventHandler = graphEventHandler;
        }


        public abstract void TryLoadConnections();

        public virtual bool AreNeighbours(int node1Id, int node2Id)
        {
            TryLoadConnections();

            return _memoryConnectionManager.AreNeighbours(node1Id, node2Id);
        }

        public abstract void Connect(int node1Id, int node2Id);

        public abstract void DeleteFromNode(int nodeId);

        public abstract void Disconnect(int node1Id, int node2Id);

        public virtual IEnumerable<INodeToNodeConnector> GetAll(int skip, int count)
        {
            TryLoadConnections();

            return _memoryConnectionManager.GetAll(skip, count);
        }


        public virtual IEnumerable<int> GetNeighbourIds(int nodeId, int skip, int count)
        {
            TryLoadConnections();

            return _memoryConnectionManager.GetNeighbourIds(nodeId, skip, count);
        }

        public virtual int GetNeighbourCount(int nodeId)
        {
            TryLoadConnections();

            return _memoryConnectionManager.GetNeighbourCount(nodeId);
        }

        public virtual IGraphInfo GetGraphInfo()
        {
            TryLoadConnections();

            return _memoryConnectionManager.GetGraphInfo();
        }
    }
}