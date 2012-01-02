using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class SearchFormHandler : ContentHandler
    {
        public SearchFormHandler()
        {
            // Maybe better with migrations?
            Filters.Add(new ActivatingFilter<SearchFormPart>("AssociativySearchForm"));
        }
    }
}