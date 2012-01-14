using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.Drivers
{
    [OrchardFeature("Associativy")]
    // This driver's sole purpose is to enable "casting" (.As<>()) of nodes to AssociativyNodeLabelPart
    public class AssociativyNodeLabelPartDriver : ContentPartDriver<AssociativyNodeLabelPart>
    {
        protected override string Prefix
        {
            get { return "Associativy.NodeLabelPart"; }
        }
    }
}