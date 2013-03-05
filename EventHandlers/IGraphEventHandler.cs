using Associativy.GraphDiscovery;
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
        void NodeAdded(IGraphDescriptor graphDescriptor, IContent node);

        void NodeRemoved(IGraphDescriptor graphDescriptor, IContent node);

        void NodeChanged(IGraphDescriptor graphDescriptor, IContent node);

        void ConnectionAdded(IGraphDescriptor graphDescriptor, int nodeId1, int nodeId2);

        void ConnectionsDeletedFromNode(IGraphDescriptor graphDescriptor, int nodeId);

        void ConnectionDeleted(IGraphDescriptor graphDescriptor, int nodeId1, int nodeId2);

        /// <summary>
        /// Gets called when the associative graph has changed (meaning any change listed here).
        /// </summary>
        void Changed(IGraphDescriptor graphDescriptor);
    }
}
