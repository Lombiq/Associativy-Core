using System;
using System.Collections.Generic;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeHandler : ContentHandler
    {
        public AssociativyNodeHandler(IAssociativyContextLocator contextLocator, IAssociativeGraphEventHandler graphEventHandler)
        {
            var associativyContexts = new Dictionary<string, IAssociativyContext[]>();

            // OnActivated runs every time the part is used in a content item, also if the content item was just instantiated.
            // OnLoading/OnLoaded runs only if the content item is already persisted.
            OnActivated<AssociativyNodePart>((context, part) =>
            {
                IAssociativyContext[] currentAssociativyContexts;
                if (!associativyContexts.TryGetValue(context.ContentType, out currentAssociativyContexts))
                {
                    currentAssociativyContexts = associativyContexts[context.ContentType] = contextLocator.GetContextsForContentType(context.ContentType);
                }
                part.ActiveContexts = currentAssociativyContexts;
            });

            OnCreated<AssociativyNodePart>((context, part) =>
            {
                InvokeEventHandlerWithContexts(associativyContexts[context.ContentType], (associativyContext) => graphEventHandler.NodeAdded(context.ContentItem, associativyContext));
            });

            OnRemoved<AssociativyNodePart>((context, part) =>
            {
                InvokeEventHandlerWithContexts(associativyContexts[context.ContentType], (associativyContext) => graphEventHandler.NodeRemoved(context.ContentItem, associativyContext));
            });
        }

        private void InvokeEventHandlerWithContexts(IAssociativyContext[] associativyContexts, Action<IAssociativyContext> eventHandler)
        {
            foreach (var context in associativyContexts)
            {
                eventHandler(context);
            }
        }
    }
}