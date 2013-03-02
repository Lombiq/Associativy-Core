using Associativy.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Associativy.Migrations
{
    [OrchardFeature("Associativy")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Label") // Labels can't be unique, as all graphs' nodes' labels are stored here
                    .Column<string>("UpperInvariantLabel")
                ).AlterTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table
                    .CreateIndex("UpperInvariantLabel", new string[] { "UpperInvariantLabel" })
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeLabelPart).Name, part => part.Attachable());
            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeTitleLabelPart).Name, part => part.Attachable());


            return 2;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeTitleLabelPart).Name, part => part.Attachable());


            return 2;
        }
    }
}