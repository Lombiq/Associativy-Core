using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.ViewModels;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;

namespace Associativy.Settings
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelTypePartSettings
    {
        public string DefaultLabelPattern { get; set; }

        public AssociativyNodeLabelTypePartSettings()
        {
            DefaultLabelPattern = "{Content.DisplayText}";
        }
    }

    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelSettingsHooks : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition)
        {
            if (definition.PartDefinition.Name != "AssociativyNodeLabelPart")
                yield break;

            var model = definition.Settings.GetModel<AssociativyNodeLabelTypePartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.Name != "AssociativyNodeLabelPart")
                yield break;

            var model = new AssociativyNodeLabelTypePartSettings();
            updateModel.TryUpdateModel(model, "AssociativyNodeLabelTypePartSettings", null, null);
            builder.WithSetting("AssociativyNodeLabelTypePartSettings.DefaultLabelPattern", !String.IsNullOrWhiteSpace(model.DefaultLabelPattern) ? model.DefaultLabelPattern : null);
            yield return DefinitionTemplate(model);
        }
    }
}