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
    public interface IGraphEventHandler : IEventHandler
    {
        void NodeAdded(IContent node, IGraphDescriptor graphDescriptor);

        void NodeRemoved(IContent node, IGraphDescriptor graphDescriptor);

        void NodeChanged(IContent node, IGraphDescriptor graphDescriptor);

        void ConnectionAdded(int nodeId1, int nodeId2, IGraphDescriptor graphDescriptor);

        void ConnectionsDeletedFromNode(int nodeId, IGraphDescriptor graphDescriptor);

        void ConnectionDeleted(int nodeId1, int nodeId2, IGraphDescriptor graphDescriptor);

        /// <summary>
        /// Gets called when the associative graph has changed.
        /// </summary>
        void Changed(IGraphDescriptor graphDescriptor);
    }
}
