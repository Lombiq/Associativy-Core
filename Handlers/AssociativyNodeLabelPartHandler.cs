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
                part.Label = context.ContentItem.As<ITitleAspect>().Title;
            });

            OnUpdateEditorShape<AssociativyNodeLabelPart>((context, part) =>
            {
                part.Label = context.ContentItem.As<ITitleAspect>().Title;
            });
        }
    }
}