using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public abstract class AssociativyNodeHandler : ContentHandler
    {
        public AssociativyNodeHandler(IAssociativyContext associativyContext, IAssociativeGraphEventHandler associativeGraphEventHandler)
        {
            OnActivated<AssociativyNodePart>((context, part) =>
            {
                if (associativyContext.ContentTypes.Contains(context.ContentType))
                {
                    part.CurrentContext = associativyContext;
                }
            });

            OnCreated<AssociativyNodePart>((context, part) =>
            {
                associativeGraphEventHandler.NodeAdded(context.ContentItem, associativyContext);
            });

            OnRemoved<AssociativyNodePart>((context, part) =>
            {
                associativeGraphEventHandler.NodeRemoved(context.ContentItem, associativyContext);
            });
        }
    }
}