using Orchard.ContentManagement.Aspects;

namespace Associativy.Models
{
    public class AssociativyNodeTitleLabelPart : AssociativyNodeLabelPart, ITitleAspect
    {
        public string Title { get { return Label; } }
    }
}