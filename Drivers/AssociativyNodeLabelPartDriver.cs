using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.Drivers
{
    [OrchardFeature("Associativy")]
    // This driver's purpose is also to enable "casting" (.As<>()) of nodes to AssociativyNodeLabelPart
    public class AssociativyNodeLabelPartDriver : ContentPartDriver<AssociativyNodeLabelPart>
    {
        protected override string Prefix
        {
            get { return "Associativy.NodeLabelPart"; }
        }

        protected override void Exporting(AssociativyNodeLabelPart part, Orchard.ContentManagement.Handlers.ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Label", part.Label);
        }

        protected override void Importing(AssociativyNodeLabelPart part, Orchard.ContentManagement.Handlers.ImportContentContext context)
        {
            part.Label = context.Attribute(part.PartDefinition.Name, "Label");
        }
    }
}