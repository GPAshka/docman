using FluentMigrator;

namespace Docman.Infrastructure.PostgreSql.Migrations
{
    [Migration(20201112132800)]
    public class AddUserIdColumnToDocumentsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Documents").InSchema("documents")
                .AddColumn("UserId").AsGuid().Nullable();
        }

        public override void Down()
        {
            Delete.Column("UserId").FromTable("Documents").InSchema("documents");
        }
    }
}