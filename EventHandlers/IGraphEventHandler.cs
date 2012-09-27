﻿using Associativy.GraphDiscovery;
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
        void NodeAdded(IGraphContext graphContext, IContent node);

        void NodeRemoved(IGraphContext graphContext, IContent node);

        void NodeChanged(IGraphContext graphContext, IContent node);

        void ConnectionAdded(IGraphContext graphContext, int nodeId1, int nodeId2);

        void ConnectionsDeletedFromNode(IGraphContext graphContext, int nodeId);

        void ConnectionDeleted(IGraphContext graphContext, int nodeId1, int nodeId2);

        /// <summary>
        /// Gets called when the associative graph has changed (meaning any change listed here).
        /// </summary>
        void Changed(IGraphContext graphContext);
    }
}
