using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Diagnostics;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeHandler : ContentHandler
    {
        private readonly IDictionary<string, IList<IGraphDescriptor>> _graphDescriptors;
        private readonly IGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IGraphDescriptorLocator graphDescriptorLocator, IGraphEventHandler graphEventHandler)
        {
            _graphDescriptors = graphDescriptorLocator.FindGraphDescriptorsByRegisteredContentTypes();
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            if (!_graphDescriptors.ContainsKey(context.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_graphDescriptors[context.ContentType], (graphDescriptor) => _graphEventHandler.NodeAdded(context.ContentItem, graphDescriptor));
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

        private void InvokeEventHandlerWithGraphDescriptors(IList<IGraphDescriptor> graphDescriptors, Action<IGraphDescriptor> eventHandler)
        {
            foreach (var graphDescriptor in graphDescriptors)
            {
                eventHandler(graphDescriptor);
            }
        }
    }
}