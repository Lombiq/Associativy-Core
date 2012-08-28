using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Associativy.Drivers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPartDriver : ContentPartDriver<AssociativyNodeLabelPart>
    {
        protected override string Prefix
        {
            get { return "Associativy.NodeLabelPart"; }
        }

        // GET
        protected override DriverResult Editor(AssociativyNodeLabelPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_AssociativyNodeLabel_Edit",
                () => shapeHelper.EditorTemplate(
                        TemplateName: "Parts.AssociativyNodeLabel",
                        Model: part,
                        Prefix: Prefix));
        }

        // POST
        protected override DriverResult Editor(AssociativyNodeLabelPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Exporting(AssociativyNodeLabelPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Label", part.Label);
        }

        protected override void Importing(AssociativyNodeLabelPart part, ImportContentContext context)
        {
            context.ImportAttribute(part.PartDefinition.Name, "Label", value => part.Label = value);
        }
    }
}