using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Diagnostics;
using Associativy.GraphDiscovery;
using System.Linq;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
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
            TryInvokeEventHandler(context.ContentType, (graphContext, graphDescriptor) => _graphEventHandler.Value.NodeAdded(graphContext, context.ContentItem));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext, graphDescriptor) => _graphEventHandler.Value.NodeChanged(graphContext, context.ContentItem));
        }

        protected override void Removed(RemoveContentContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext, graphDescriptor) =>
                {
                    graphDescriptor.ConnectionManager.DeleteFromNode(graphContext, context.ContentItem);
                    _graphEventHandler.Value.NodeRemoved(graphContext, context.ContentItem);
                });
        }

        private void TryInvokeEventHandler(string contentType, Action<IGraphContext, GraphDescriptor> eventHandler)
        {
            var context = new GraphContext { ContentTypes = new string[] { contentType }};
            var descriptors = _graphManager.Value.FindGraphs(context);

            foreach (var descriptor in descriptors)
            {
                // descriptor.ProduceContext() could be erroneous as the context with only the current content type is needed,
                // not all content types stored by the graph.
                context.GraphName = descriptor.GraphName;
                eventHandler(context, descriptor);
            }
        }
    }
}