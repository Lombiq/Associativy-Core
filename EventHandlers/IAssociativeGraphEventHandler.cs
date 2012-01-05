using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Events;

namespace Associativy.EventHandlers
{
    /// <summary>
    /// Interface for graph event handlers.
    /// 
    /// When an event occurs, all types implementing this interface will get their respective method called.
    /// </summary>
    public interface IAssociativeGraphEventHandler : IEventHandler
    {
        void NodeAdded(IContent node, IAssociativyContext context);

        void NodeRemoved(IContent node, IAssociativyContext context);

        void NodeChanged(IContent node, IAssociativyContext context);

        void ConnectionAdded(int nodeId1, int nodeId2, IAssociativyContext context);

        void ConnectionsDeletedFromNode(int nodeId, IAssociativyContext context);

        void ConnectionDeleted(int connectionId, IAssociativyContext context);

        /// <summary>
        /// Gets called when the associative graph has changed.
        /// </summary>
        void Changed(IAssociativyContext context);
    }
}
