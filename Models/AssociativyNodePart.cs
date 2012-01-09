using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    /// <summary>
    /// This part is to mark content types that could be nodes of a graph.
    /// </summary>
    /// <remarks>
    /// Could be that It's not needed, remove if necessary.
    /// </remarks>
    [OrchardFeature("Associativy")]
    public class AssociativyNodePart : ContentPart
    {
        // A content item can be part of multiple graphs
        public IAssociativyContext[] ActiveContexts { get; set; }
    }
}