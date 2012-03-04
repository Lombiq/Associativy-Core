using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Associativy.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;
using Orchard.Core.Common.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Tokens;
using System;
using System.Linq;
using Associativy.Settings;
using System.Collections.Generic;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyNodeLabelPartHandler : ContentHandler
    {
        public AssociativyNodeLabelPartHandler(
            IRepository<AssociativyNodeLabelPartRecord> repository,
            Lazy<ITokenizer> tokenizer,
            Lazy<IContentDefinitionManager> contentDefinitionManager)
        {
            Filters.Add(StorageFilter.For(repository));

            // OnUpdateEditorShape is not suitable as title is not filled there yet
            OnPublished<AssociativyNodeLabelPart>((context, part) =>
            {
                if (!String.IsNullOrWhiteSpace(part.Label)) return;

                var settings = contentDefinitionManager.Value
                    .GetTypeDefinition(context.ContentType)
                    .Parts.First(x => x.PartDefinition.Name == "AssociativyNodeLabelPart")
                    .Settings.GetModel<AssociativyNodeLabelTypePartSettings>();

                // Setting label to the tokenized value
                part.Label = tokenizer.Value.Replace(
                    settings.DefaultLabelPattern, 
                    new Dictionary<string, object> { { "Content", context.ContentItem } }, 
                    new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
            });
        }
    }
}