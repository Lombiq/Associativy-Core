using System;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Models
{
    public interface IAssociativyContext
    {
        LocalizedString Name { get; }
        string TechnicalName { get; }
    }

    public interface IAssociativyContext<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IAssociativyContext
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
    }
}
