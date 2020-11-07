using FluentMigrator;

namespace Docman.Infrastructure.PostgreSql.Migrations
{
    [Migration(20201107141900)]
    public class CreateFilesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Files")
                .InSchema("documents")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("DocumentId").AsGuid().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Files").InSchema("documents");
        }
    }
}