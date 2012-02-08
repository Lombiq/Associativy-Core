using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;
using Orchard.Core.Common.Models;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPartHandler : ContentHandler
    {
        public AssociativyNodeLabelPartHandler(IRepository<AssociativyNodeLabelPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            // OnUpdateEditorShape is not suitable as title is not filled there yet
            OnPublished<AssociativyNodeLabelPart>((context, part) =>
            {
                // .Has<> doesn't work here
                var titleAspect = context.ContentItem.As<ITitleAspect>();
                if (titleAspect != null) part.Label = context.ContentItem.As<ITitleAspect>().Title;
            });
        }
    }
}