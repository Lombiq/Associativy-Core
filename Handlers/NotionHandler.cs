
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using JetBrains.Annotations;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionHandler : ContentHandler
    {
        public NotionHandler(IRepository<NotionPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<NotionPart>("Notion"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}