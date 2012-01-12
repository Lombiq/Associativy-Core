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
        private readonly IDictionary<string, IList<IAssociativyContext>> _associativyContexts;
        private readonly IAssociativeGraphEventHandler _graphEventHandler;

        public AssociativyNodeHandler(IAssociativyContextLocator contextLocator, IAssociativeGraphEventHandler graphEventHandler)
        {
            _associativyContexts = contextLocator.GetContextsByRegisteredContentTypes();
            _graphEventHandler = graphEventHandler;
        }

        protected override void Created(CreateContentContext context)
        {
            if (!_associativyContexts.ContainsKey(context.ContentType)) return;

            InvokeEventHandlerWithContexts(_associativyContexts[context.ContentType], (associativyContext) => _graphEventHandler.NodeAdded(context.ContentItem, associativyContext));
        }

        protected override void UpdateEditorShape(UpdateEditorContext context)
        {
            if (!_associativyContexts.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithContexts(_associativyContexts[context.ContentItem.ContentType], (associativyContext) => _graphEventHandler.NodeChanged(context.ContentItem, associativyContext));
        }

        protected override void Removed(RemoveContentContext context)
        {
            if (!_associativyContexts.ContainsKey(context.ContentItem.ContentType)) return;

            InvokeEventHandlerWithContexts(_associativyContexts[context.ContentItem.ContentType], (associativyContext) => _graphEventHandler.NodeRemoved(context.ContentItem, associativyContext));
        }

        private void InvokeEventHandlerWithContexts(IList<IAssociativyContext> associativyContexts, Action<IAssociativyContext> eventHandler)
        {
            foreach (var context in associativyContexts)
            {
                eventHandler(context);
            }
        }
    }
}