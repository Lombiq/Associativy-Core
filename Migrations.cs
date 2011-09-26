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
    [OrchardFeature("Associativy")]
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("NodePartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Label")
                );

            //SchemaBuilder.CreateForeignKey("NodeRecord_Id", 
            SchemaBuilder.CreateTable("NodeToNodeRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("NodeRecord1Id")
                    .Column<int>("NodeRecord2Id")
                );



            return 1;
        }
    }
}