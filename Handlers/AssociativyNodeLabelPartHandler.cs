using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPartHandler : ContentHandler
    {
        public AssociativyNodeLabelPartHandler(IRepository<AssociativyNodeLabelPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnCreating<AssociativyNodeLabelPart>((context, part) =>
            {
                // .Has<> doesn't work here
                var titleAspect = context.ContentItem.As<ITitleAspect>();
                if (titleAspect != null) part.Label = titleAspect.Title;
            });

            OnUpdateEditorShape<AssociativyNodeLabelPart>((context, part) =>
            {
                var titleAspect = context.ContentItem.As<ITitleAspect>();
                if (titleAspect != null) part.Label = context.ContentItem.As<ITitleAspect>().Title;
            });
        }
    }
}