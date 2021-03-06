﻿using Associativy.Models;
using Orchard.Data.Migration.Schema;

namespace Associativy.Extensions
{
    public static class SchemaBuilderExtensions
    {
        public static SchemaBuilder CreateNodeToNodeConnectorRecordTable<TNodeToNodeConnectorRecord>(this SchemaBuilder schemaBuilder)
            where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
        {
            schemaBuilder.CreateTable(typeof(TNodeToNodeConnectorRecord).Name,
                table => table
                    .NodeConnectorRecord()
                );

            // TODO: TEST INDICES as data grows
            schemaBuilder.AlterTable(typeof(TNodeToNodeConnectorRecord).Name,
                table => table
                    .CreateIndex("Connection", new string[] { "Node1Id", "Node2Id" })
                    // These are maybe not needed
                    // SELECT this_.Id as Id0_0_, this_.Record1Id as Record2_0_0_, this_.Record2Id as Record3_0_0_ FROM Associativy_Notions_NotionToNotionConnectorRecord this_ WHERE this_.Record1Id = 22
                    //CreateIndex("Record1", new string[] { "Record1Id" });
                    //CreateIndex("Record2", new string[] { "Record2Id" });
                );

            return schemaBuilder;
        }
    }
}