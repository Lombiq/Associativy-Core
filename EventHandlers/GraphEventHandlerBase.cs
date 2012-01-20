using Associativy.Models;
using Orchard.ContentManagement;
using Associativy.GraphDiscovery;

namespace Associativy.EventHandlers
{
    public abstract class GraphEventHandlerBase : IGraphEventHandler
    {
        public virtual void NodeAdded(IGraphContext graphContext, IContent node)
        {
            Changed(graphContext);
        }

        public virtual void NodeRemoved(IGraphContext graphContext, IContent node)
        {
            Changed(graphContext);
        }

        public virtual void NodeChanged(IGraphContext graphContext, IContent node)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionAdded(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionsDeletedFromNode(IGraphContext graphContext, int nodeId)
        {
            Changed(graphContext);
        }

        public virtual void ConnectionDeleted(IGraphContext graphContext, int nodeId1, int nodeId2)
        {
            Changed(graphContext);
        }

        public abstract void Changed(IGraphContext graphContext);
    }
}