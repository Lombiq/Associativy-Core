using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Handlers;
using Associativy.Models;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public abstract class AssociativyNodeHandler : ContentHandler
    {
        //protected virtual string FrontendEngine
        //{
        //    get { return ""; }
        //}

        public AssociativyNodeHandler(IAssociativyContext associativyContext)
        {
            OnActivated<AssociativyNodePart>((context, part) =>
            {
                if (associativyContext.ContentTypes.Contains(context.ContentType))
                {
                    part.CurrentContext = associativyContext;
                }
            });
        }
    }
}