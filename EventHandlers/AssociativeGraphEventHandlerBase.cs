using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.ContentManagement;

namespace Associativy.EventHandlers
{
    public abstract class AssociativeGraphEventHandlerBase : IAssociativeGraphEventHandler
    {
        public virtual void NodeAdded(IContent node, IAssociativyContext context)
        {
            Changed(context);
        }

        public virtual void NodeRemoved(IContent node, IAssociativyContext context)
        {
            Changed(context);
        }

        public virtual void NodeChanged(IContent node, IAssociativyContext context)
        {
            Changed(context);
        }

        public virtual void ConnectionAdded(int nodeId1, int nodeId2, IAssociativyContext context)
        {
            Changed(context);
        }

        public virtual void ConnectionsDeletedFromNode(int nodeId, IAssociativyContext context)
        {
            Changed(context);
        }

        public virtual void ConnectionDeleted(int connectionId, IAssociativyContext context)
        {
            Changed(context);
        }

        public abstract void Changed(IAssociativyContext context);
    }
}