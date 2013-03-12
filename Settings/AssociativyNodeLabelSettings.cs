using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Associativy.Settings
{
    public class AssociativyNodeLabelTypePartSettings
    {
        public string DefaultLabelPattern { get; set; }
        public string ContentType { get; set; }

        public AssociativyNodeLabelTypePartSettings()
        {
            DefaultLabelPattern = "{Content.DisplayText}";
        }
    }

    public class AssociativyNodeLabelSettingsHooks : ContentDefinitionEditorEventsBase
    {
        private string _contentType;

        public override IEnumerable<TemplateViewModel> TypeEditor(ContentTypeDefinition definition)
        {
            _contentType = definition.Name;
            yield break;
        }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition)
        {
            if (definition.PartDefinition.Name != "AssociativyNodeLabelPart")
                yield break;

            var model = definition.Settings.GetModel<AssociativyNodeLabelTypePartSettings>();
            model.ContentType = _contentType;

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