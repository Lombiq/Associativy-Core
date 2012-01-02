using Orchard.Data.Migration.Schema;

namespace Associativy.Extensions
{
    public static class CreateTableCommandExtensions
    {
        public static CreateTableCommand NodeConnectorRecord(this CreateTableCommand table)
        {
            table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("Node1Id", column => column.NotNull())
                .Column<int>("Node2Id", column => column.NotNull());

            return table;
        }
    }
}