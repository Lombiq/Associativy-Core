using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodePart : ContentPart
    {
        // A content item can be part of multiple graphs
        public IAssociativyContext[] ActiveContexts { get; set; }
    }
}