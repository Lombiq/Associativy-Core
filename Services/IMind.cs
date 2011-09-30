﻿using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;
using QuickGraph;

namespace Associativy.Services
{
    public interface IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociations(int zoomLevel = 0);
        UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<TNodePart> terms, bool simpleAlgorithm = false);
    }
}
