using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Diagnostics;
using Associativy.GraphDescription;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeHandler : ContentHandler
    {
        private readonly IGraphDescriptorManager _graphDescriptorManager;
        private readonly IGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IGraphDescriptorManager graphDescriptorManager, IGraphEventHandler graphEventHandler)
        {
            _graphDescriptorManager = graphDescriptorManager;
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            TryInvokeEventHandler(context.ContentType, (graphContext) => _graphEventHandler.NodeAdded(context.ContentItem, graphContext));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            if (!_graphDescriptors.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_graphDescriptors[context.ContentItem.ContentType], (graphDescriptor) => _graphEventHandler.NodeChanged(context.ContentItem, graphDescriptor));
        }

        protected override void Removed(RemoveContentContext context)
        {
            if (!_graphDescriptors.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_graphDescriptors[context.ContentItem.ContentType], (graphDescriptor) => _graphEventHandler.NodeRemoved(context.ContentItem, graphDescriptor));

            foreach (var graphDescriptor in _graphDescriptors[context.ContentItem.ContentType])
            {
                graphDescriptor.ConnectionManager.DeleteFromNode(context.ContentItem);
            }
        }

        private void TryInvokeEventHandler(string contentType, Action<IGraphContext> eventHandler)
        {
            IEnumerable<IGraphDescriptor> descriptors;
            if (!_graphDescriptorManager.TryFindDescriptorsForContentType(new ContentContext(contentType), out descriptors)) return;

            foreach (var descriptor in descriptors)
            {
                eventHandler(new GraphContext(descriptor.GraphName));
            }
        }
    }
}