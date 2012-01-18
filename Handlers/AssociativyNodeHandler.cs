using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Diagnostics;
using Associativy.GraphDiscovery;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeHandler : ContentHandler
    {
        private readonly IGraphProviderManager _graphProviderManager;
        private readonly IGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IGraphProviderManager graphProviderManager, IGraphEventHandler graphEventHandler)
        {
            _graphProviderManager = graphProviderManager;
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            TryInvokeEventHandler(context.ContentType, (graphContext) => _graphEventHandler.NodeAdded(context.ContentItem, graphContext));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext) => _graphEventHandler.NodeChanged(context.ContentItem, graphContext));
        }

        protected override void Removed(RemoveContentContext context)
        {
            TryInvokeEventHandler(context.ContentItem.ContentType, (graphContext) =>
                {
                    _graphEventHandler.NodeRemoved(context.ContentItem, graphContext);
                });

            // For now connections should remanin intact
            //foreach (var graphDescriptor in _graphDescriptors[context.ContentItem.ContentType])
            //{
            //    graphDescriptor.ConnectionManager.DeleteFromNode(context.ContentItem);
            //}
        }

        private void TryInvokeEventHandler(string contentType, Action<IGraphContext> eventHandler)
        {
            IEnumerable<IGraphProvider> descriptors;
            if (!_graphProviderManager.TryFindProvidersForContentType(new ContentContext(contentType), out descriptors)) return;

            foreach (var descriptor in descriptors)
            {
                eventHandler(new GraphContext(descriptor.GraphName));
            }
        }
    }
}