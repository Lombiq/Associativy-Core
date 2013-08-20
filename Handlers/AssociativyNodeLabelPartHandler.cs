using System;
using System.Collections.Generic;
using Associativy.Models;
using Associativy.Settings;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Tokens;

namespace Associativy.Handlers
{
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

                var settings = part.Settings.GetModel<AssociativyNodeLabelTypePartSettings>();

                // Setting label to the tokenized value
                part.Label = tokenizer.Value.Replace(
                    settings.DefaultLabelPattern, 
                    new Dictionary<string, object> { { "Content", context.ContentItem } }, 
                    new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
            });
        }
    }
}