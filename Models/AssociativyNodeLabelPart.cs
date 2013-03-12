using Orchard.ContentManagement;

namespace Associativy.Models
{
    public class AssociativyNodeLabelPart : ContentPart<AssociativyNodeLabelPartRecord>, IAssociativyNodeLabelAspect
    {
        public string Label
        {
            get { return Record.Label; }
            set
            {
                if (value == null) return;
                Record.Label = value;
                Record.UpperInvariantLabel = value.ToUpperInvariant();
            }
        }
    }
}