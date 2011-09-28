using System;
using System.Collections.Generic;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    public interface IAssociativyService<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IDependency // Maybe? ISingletonDependency
        where TNodePart : NodePart<TNodePartRecord>
        where TNodePartRecord : NodePartRecord //ContentPartRecord
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        //IList<NodePartRecord> GetNeighbours(int nodeId);
    }
}
