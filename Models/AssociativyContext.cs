using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the context in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    public abstract class AssociativyContext : IAssociativyContext
    {
        protected readonly LocalizedString _name;
        public LocalizedString Name
        {
            get { return _name; }
        }

        protected readonly string _technicalName;
        public string TechnicalName
        {
            get { return _technicalName; }
        }

        public Localizer T { get; set; }

        public AssociativyContext()
        {
            T = NullLocalizer.Instance;
        }
    }

    public abstract class AssociativyContext<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        : AssociativyContext, IAssociativyContext<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        
    }
}