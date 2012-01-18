using Associativy.Models;
using Orchard.ContentManagement;
using Associativy.GraphDiscovery;

namespace Associativy.EventHandlers
{
    public abstract class GraphEventHandlerBase : IGraphEventHandler
    {
        public virtual void NodeAdded(IContent node, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public virtual void NodeRemoved(IContent node, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public virtual void NodeChanged(IContent node, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionAdded(int nodeId1, int nodeId2, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionsDeletedFromNode(int nodeId, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionDeleted(int nodeId1, int nodeId2, IGraphContext graphContext)
        {
            Changed(graphContext);
        }

        public abstract void Changed(IGraphContext graphContext);
    }
}