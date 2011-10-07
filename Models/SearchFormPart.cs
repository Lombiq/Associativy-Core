using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class SearchFormPart : ContentPart
    {
        public string Terms { get; set; }
    }
}