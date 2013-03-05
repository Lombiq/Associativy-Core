using Associativy.GraphDiscovery;
using Orchard.ContentManagement;

namespace Associativy.EventHandlers
{
    public abstract class GraphEventHandlerBase : IGraphEventHandler
    {
        public virtual void NodeAdded(IGraphDescriptor graphDescriptor, IContent node)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeRemoved(IGraphDescriptor graphDescriptor, IContent node)
        {
            Changed(graphDescriptor);
        }

        public virtual void NodeChanged(IGraphDescriptor graphDescriptor, IContent node)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionAdded(IGraphDescriptor graphDescriptor, int nodeId1, int nodeId2)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionsDeletedFromNode(IGraphDescriptor graphDescriptor, int nodeId)
        {
            Changed(graphDescriptor);
        }

        public virtual void ConnectionDeleted(IGraphDescriptor graphDescriptor, int nodeId1, int nodeId2)
        {
            Changed(graphDescriptor);
        }

        public abstract void Changed(IGraphDescriptor graphDescriptor);
    }
}