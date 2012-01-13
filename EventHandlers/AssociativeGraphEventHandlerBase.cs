using Associativy.Models;
using Orchard.ContentManagement;

namespace Associativy.EventHandlers
{
    public abstract class AssociativeGraphEventHandlerBase : IAssociativeGraphEventHandler
    {
        public virtual void NodeAdded(IContent node, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeRemoved(IContent node, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeChanged(IContent node, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionAdded(int nodeId1, int nodeId2, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionsDeletedFromNode(int nodeId, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionDeleted(int connectionId, IAssociativyGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public abstract void Changed(IAssociativyGraphDescriptor graphDescriptor);
    }
}