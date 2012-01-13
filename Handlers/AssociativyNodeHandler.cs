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
        private readonly IDictionary<string, IList<IAssociativyGraphDescriptor>> _associativyGraphDescriptors;
        private readonly IAssociativeGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IAssociativyGraphDescriptorLocator graphDescriptorLocator, IAssociativeGraphEventHandler graphEventHandler)
        {
            _associativyGraphDescriptors = graphDescriptorLocator.GetGraphDescriptorsByRegisteredContentTypes();
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            if (!_associativyGraphDescriptors.ContainsKey(context.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_associativyGraphDescriptors[context.ContentType], (associativyGraphDescriptor) => _graphEventHandler.NodeAdded(context.ContentItem, associativyGraphDescriptor));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            if (!_associativyGraphDescriptors.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_associativyGraphDescriptors[context.ContentItem.ContentType], (associativyGraphDescriptor) => _graphEventHandler.NodeChanged(context.ContentItem, associativyGraphDescriptor));
        }

        protected override void Removed(RemoveContentContext context)
        {
            if (!_associativyGraphDescriptors.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithGraphDescriptors(_associativyGraphDescriptors[context.ContentItem.ContentType], (associativyGraphDescriptor) => _graphEventHandler.NodeRemoved(context.ContentItem, associativyGraphDescriptor));
        }

        private void InvokeEventHandlerWithGraphDescriptors(IList<IAssociativyGraphDescriptor> associativyGraphDescriptors, Action<IAssociativyGraphDescriptor> eventHandler)
        {
            foreach (var graphDescriptor in associativyGraphDescriptors)
            {
                eventHandler(graphDescriptor);
            }
        }
    }
}