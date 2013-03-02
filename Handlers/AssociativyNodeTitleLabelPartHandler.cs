using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeTitleLabelPartHandler : ContentHandler
    {
        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var part = context.ContentItem.As<AssociativyNodeTitleLabelPart>();

            if (part != null) context.Metadata.DisplayText = part.Title;
        }
    }
}