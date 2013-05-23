using Associativy.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;

namespace Associativy.Migrations
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Label") // Labels can't be unique, as all graphs' nodes' labels are stored here
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeLabelPart).Name, 
                part => part
                    .Attachable()
                    .WithDescription("Stores a label used when the node of an Associativy graph is displayed or searched."));

            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeTitleLabelPart).Name, 
                part => part
                    .Attachable()
                    .WithDescription("Stores a label used when the node of an Associativy graph is displayed or searched. The label functions as the title of the item too."));


            return 4;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeTitleLabelPart).Name, part => part.Attachable());


            return 2;
        }

        public int UpdateFrom2()
        {
            // Can't drop column here, must do in a separate batch
            SchemaBuilder.AlterTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table.DropIndex("UpperInvariantLabel"));


            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table.DropColumn("UpperInvariantLabel"));

            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeLabelPart).Name,
                part => part
                    .WithDescription("Stores a label used when the node of an Associativy graph is displayed or searched."));

            ContentDefinitionManager.AlterPartDefinition(typeof(AssociativyNodeTitleLabelPart).Name,
                part => part
                    .WithDescription("Stores a label used when the node of an Associativy graph is displayed or searched. The label functions as the title of the item too."));


            return 4;
        }
    }
}