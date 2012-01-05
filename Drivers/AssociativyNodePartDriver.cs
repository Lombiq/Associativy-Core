using Associativy.EventHandlers;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Associativy.Drivers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodePartDriver : ContentPartDriver<AssociativyNodePart>
    {
        private readonly IAssociativeGraphEventHandler _associativeGraphEventHandler;

        protected override string Prefix
        {
            get { return "Associativy.NodePart"; }
        }

        public AssociativyNodePartDriver(
            IAssociativeGraphEventHandler associativeGraphEventHandler)
        {
            _associativeGraphEventHandler = associativeGraphEventHandler;
        }

        protected override DriverResult Display(AssociativyNodePart part, string displayType, dynamic shapeHelper)
        {
            return null;
        }

        // GET
        protected override DriverResult Editor(AssociativyNodePart part, dynamic shapeHelper)
        {
            return null;
            //return ContentShape("Parts_SearchForm",
            //    () => shapeHelper.EditorTemplate(
            //            TemplateName: "Parts/SearchForm",
            //            Model: part,
            //            Prefix: Prefix));
        }

        // POST
        protected override DriverResult Editor(AssociativyNodePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            _associativeGraphEventHandler.NodeChanged(part.ContentItem, part.CurrentContext);

            return Editor(part, shapeHelper);
        }
    }
}