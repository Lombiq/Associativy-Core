using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data.Migration.Schema;
using Associativy.Models;

namespace Associativy.Extensions
{
    public static class SchemaBuilderExtensions
    {
        public static SchemaBuilder CreateNodeToNodeConnectorRecordTable<TNodeToNodeConnectorRecord>(this SchemaBuilder schemaBuilder)
            where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        {
            schemaBuilder.CreateTable(typeof(TNodeToNodeConnectorRecord).Name,
                table => table
                    .NodeConnectorRecord()
                );

            // TODO: TEST INDICES as data grows
            schemaBuilder.AlterTable(typeof(TNodeToNodeConnectorRecord).Name,
                    table =>
                    {
                        table.CreateIndex("Connection", new string[] { "Node1Id", "Node2Id" });
                        // These are maybe not needed
                        // SELECT this_.Id as Id0_0_, this_.Record1Id as Record2_0_0_, this_.Record2Id as Record3_0_0_ FROM Associativy_Notions_NotionToNotionConnectorRecord this_ WHERE this_.Record1Id = 22
                        //table.CreateIndex("Record1", new string[] { "Record1Id" });
                        //table.CreateIndex("Record2", new string[] { "Record2Id" });
                    }
                );

            return schemaBuilder;
        }
    }
}