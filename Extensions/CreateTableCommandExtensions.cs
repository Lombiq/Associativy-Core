using Orchard.Data.Migration.Schema;

namespace Associativy.Extensions
{
    public static class CreateTableCommandExtensions
    {
        public static CreateTableCommand NodeConnectorRecord(this CreateTableCommand table)
        {
            table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("Record1Id", column => column.NotNull())
                .Column<int>("Record2Id", column => column.NotNull());

            return table;
        }

        public static CreateTableCommand NodePartRecord(this CreateTableCommand table)
        {
            table
                .ContentPartRecord()
                .Column<string>("Label", column => column.NotNull());

            return table;
        }
    }
}