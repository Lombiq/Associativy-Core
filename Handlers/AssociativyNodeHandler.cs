using System;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;

namespace Associativy.Handlers
{
    public class AssociativyNodeHandler : ContentHandler
    {
        private readonly Lazy<IGraphManager> _graphManager;
        private readonly Lazy<IGraphEventHandler> _graphEventHandler;


        public AssociativyNodeHandler(Lazy<IGraphManager> graphManager, Lazy<IGraphEventHandler> graphEventHandler)
        {
            _graphManager = graphManager;
            _graphEventHandler = graphEventHandler;
        }


        protected override void Created(CreateContentContext context)
        {
            TryInvokeEventHandler(context.ContentType, graphDescriptor => _graphEventHandler.Value.NodeAdded(graphDescriptor, context.ContentItem));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, graphDescriptor => _graphEventHandler.Value.NodeChanged(graphDescriptor, context.ContentItem));
        }

        protected override void Removed(RemoveContentContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, graphDescriptor =>
                {
                    graphDescriptor.Services.ConnectionManager.DeleteFromNode(context.ContentItem);
                    _graphEventHandler.Value.NodeRemoved(graphDescriptor, context.ContentItem);
                });
        }

        private void TryInvokeEventHandler(string contentType, Action<IGraphDescriptor> eventHandler)
        {
            var descriptors = _graphManager.Value.FindGraphsByContentTypes(contentType);

            foreach (var descriptor in descriptors)
            {
                // descriptor.ProduceContext() could be erroneous as the context with only the current content type is needed,
                // not all content types stored by the graph.
                eventHandler(descriptor);
            }
        }
    }
}