using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.Events;
using Associativy.GraphDescription;

namespace Associativy.EventHandlers
{
    /// <summary>
    /// Interface for graph event handlers.
    /// 
    /// When an event occurs, all types implementing this interface will get their respective method called.
    /// </summary>
    public interface IGraphEventHandler : IEventHandler
    {
        void NodeAdded(IContent node, IGraphContext graphContext);

        void NodeRemoved(IContent node, IGraphContext graphContext);

        void NodeChanged(IContent node, IGraphContext graphContext);

        void ConnectionAdded(int nodeId1, int nodeId2, IGraphContext graphContext);

        void ConnectionsDeletedFromNode(int nodeId, IGraphContext graphContext);

        void ConnectionDeleted(int nodeId1, int nodeId2, IGraphContext graphContext);

        /// <summary>
        /// Gets called when the associative graph has changed.
        /// </summary>
        void Changed(IGraphContext graphContext);
    }
}
