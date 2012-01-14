using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData.Builders;
using Associativy.Models;

namespace Associativy.Extensions
{
    public static class ContentTypeDefinitionBuilderExtensions
    {
        public static ContentTypeDefinitionBuilder WithLabel(this ContentTypeDefinitionBuilder builder)
        {
            return builder.WithPart(typeof(AssociativyNodeLabelPart).Name);
        }
    }
}