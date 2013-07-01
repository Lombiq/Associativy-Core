using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.ContentManagement.Handlers;

namespace Associativy.Handlers
{
    public class AssociativyNodeLabelAspectHandler : ContentHandler
    {
        public AssociativyNodeLabelAspectHandler()
        {
            OnIndexing<IAssociativyNodeLabelAspect>((context, part) => context.DocumentIndex.Add("nodeLabel", part.Label).RemoveTags().Analyze().Store());
        }
    }
}