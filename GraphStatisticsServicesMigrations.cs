using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Associativy
{
    [OrchardFeature("Associativy.GraphStatisticsServices")]
    public class GraphStatisticsServicesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(GraphStatisticsRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("GraphName", column => column.NotNull().Unique().WithLength(1024))
                    .Column<int>("NodeCount")
                    .Column<int>("ConnectionCount")
                    .Column<int>("CentralNodeId")
            ).AlterTable(typeof(GraphStatisticsRecord).Name,
                table => table
                    .CreateIndex("GraphName", new string[] { "GraphName" })
            );


            return 1;
        }
    }
}