using Orchard.ContentManagement;

namespace Associativy.Models
{
    public class AssociativyNodeLabelPart : ContentPart<AssociativyNodeLabelPartRecord>, IAssociativyNodeLabelAspect
    {
        public string Label
        {
            get { return Record.Label; }
            set { Record.Label = value; }
        }
    }
}