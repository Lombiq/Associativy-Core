using Associativy.Extensions;
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
                    .Column<string>("InvariantLabel")
                );

            SchemaBuilder.AlterTable(typeof(AssociativyNodeLabelPartRecord).Name,
                table => table
                    .CreateIndex("InvariantLabel", new string[] { "InvariantLabel" })
                );


            return 1;
        }

        //public int UpdateFrom1()
        //{


        //    return 2;
        //}
    }
}