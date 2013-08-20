using Associativy.Models;
using Orchard.ContentManagement.MetaData.Builders;

namespace Associativy.Extensions
{
    public static class ContentTypeDefinitionBuilderExtensions
    {
        public static ContentTypeDefinitionBuilder WithLabel(this ContentTypeDefinitionBuilder builder)
        {
            return builder.WithPart(typeof(AssociativyNodeLabelPart).Name);
        }

        public static ContentTypeDefinitionBuilder WithTitleLabel(this ContentTypeDefinitionBuilder builder)
        {
            return builder.WithPart(typeof(AssociativyNodeTitleLabelPart).Name);
        }

        public static ContentTypeDefinitionBuilder WithNodeTitleAdapter(this ContentTypeDefinitionBuilder builder)
        {
            return builder.WithPart(typeof(AssociativyNodeTitleAdapterPart).Name);
        }
    }
}