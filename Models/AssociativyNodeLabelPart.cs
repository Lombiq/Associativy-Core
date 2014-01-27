using Orchard.ContentManagement;

namespace Associativy.Models
{
    public class AssociativyNodeLabelPart : ContentPart<AssociativyNodeLabelPartRecord>, IAssociativyNodeLabelAspect
    {
        public string Label
        {
            get { return Retrieve(x => x.Label); }
            set { Store(x => x.Label, value); }
        }
    }
}