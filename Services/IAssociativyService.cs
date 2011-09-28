using System;
using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    public interface IAssociativyService<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        //IList<NodePartRecord> GetNeighbours(int nodeId);
    }
}
