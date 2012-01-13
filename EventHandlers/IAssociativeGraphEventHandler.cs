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
        void NodeAdded(IContent node, IAssociativyGraphDescriptor graphDescriptor);

        void NodeRemoved(IContent node, IAssociativyGraphDescriptor graphDescriptor);

        void NodeChanged(IContent node, IAssociativyGraphDescriptor graphDescriptor);

        void ConnectionAdded(int nodeId1, int nodeId2, IAssociativyGraphDescriptor graphDescriptor);

        void ConnectionsDeletedFromNode(int nodeId, IAssociativyGraphDescriptor graphDescriptor);

        void ConnectionDeleted(int connectionId, IAssociativyGraphDescriptor graphDescriptor);

        /// <summary>
        /// Gets called when the associative graph has changed.
        /// </summary>
        void Changed(IAssociativyGraphDescriptor graphDescriptor);
    }
}
