using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace Associativy.Handlers
{
    public class AssociativyNodeTitleLabelPartHandler : ContentHandler
    {
        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var part = context.ContentItem.As<AssociativyNodeTitleLabelPart>();

            if (part != null) context.Metadata.DisplayText = part.Title;
        }
    }
}