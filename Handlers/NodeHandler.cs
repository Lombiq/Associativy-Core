using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Associativy.Models;
using JetBrains.Annotations;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class NodeHandler : ContentHandler
    {
        public NodeHandler(IRepository<NodePartRecord> repository)
        {
            //Filters.Add(new ActivatingFilter<NodePart>("Node"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}