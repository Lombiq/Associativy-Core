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
        private readonly IGraphManager _graphManager;
        private readonly IGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IGraphManager graphManager, IGraphEventHandler graphEventHandler)
        {
            _graphManager = graphManager;
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            TryInvokeEventHandler(context.ContentType, (graphContext, graphDescriptor) => _graphEventHandler.NodeAdded(graphContext, context.ContentItem));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext, graphDescriptor) => _graphEventHandler.NodeChanged(graphContext, context.ContentItem));
        }

        protected override void Removed(RemoveContentContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext, graphDescriptor) =>
                {
                    graphDescriptor.ConnectionManager.DeleteFromNode(graphContext, context.ContentItem);
                    _graphEventHandler.NodeRemoved(graphContext, context.ContentItem);
                });
        }

        private void TryInvokeEventHandler(string contentType, Action<IGraphContext, GraphDescriptor> eventHandler)
        {
            var context = new GraphContext { ContentTypes = new string[] { contentType }};
            var descriptors = _graphManager.FindGraphs(context);

            if (descriptors.Count() == 0) return;

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