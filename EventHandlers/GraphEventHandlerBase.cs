using Associativy.Models;
using Orchard.ContentManagement;

namespace Associativy.EventHandlers
{
    public abstract class GraphEventHandlerBase : IGraphEventHandler
    {
        public virtual void NodeAdded(IContent node, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeRemoved(IContent node, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeChanged(IContent node, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionAdded(int nodeId1, int nodeId2, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionsDeletedFromNode(int nodeId, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionDeleted(int nodeId1, int nodeId2, IGraphDescriptor graphDescriptor)
        {
            Changed(graphDescriptor);
        }

        public abstract void Changed(IGraphDescriptor graphDescriptor);
    }
}