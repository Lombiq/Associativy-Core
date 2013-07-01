using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;

namespace Associativy.Models
{
    public class AssociativyNodeTitleAdapterPart : ContentPart, IAssociativyNodeLabelAspect
    {
        public string Label
        {
            get { return ContentItem.As<ITitleAspect>().Title; } // Would be nice to only have this and not the direct TitlePart reference
            set { ContentItem.As<TitlePart>().Title = value; }
        }
    }
}