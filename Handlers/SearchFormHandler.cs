using Associativy.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

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