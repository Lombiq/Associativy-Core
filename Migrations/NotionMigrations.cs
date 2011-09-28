using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Associativy.Migrations
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable("NotionPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Label")
                );

            //SchemaBuilder.CreateForeignKey("NodeRecord_Id", 
            SchemaBuilder.CreateTable("NotionToNotionConnectorRecord",
                table => table
                    .Column<long>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Record1Id")
                    .Column<int>("Record2Id")
                ).AlterTable("NotionToNotionConnectorRecord",
                table => table
                    .CreateIndex("Connection", new string[] { "Record1Id", "Record2Id" }) // EZ JÓ?
                );

            ContentDefinitionManager.AlterTypeDefinition("NotionSearchFormWidget",
                cfg => cfg
                    .WithPart("NotionSearchFormPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );


            return 1;
        }
    }
}