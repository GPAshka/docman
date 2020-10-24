using System;
using FluentMigrator;

namespace Docman.Infrastructure.PostgreSql.Migrations
{
    [Migration(20201018132100)]
    public class CreateDocumentTables : Migration
    {
        public override void Up()
        {
            Create.Schema("documents");

            Create.Table("Documents")
                .InSchema("documents")
                .WithColumn("Id").AsGuid().NotNullable()
                .WithColumn("Number").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("Status").AsString().NotNullable()
                .WithColumn("ApprovalComment").AsString().Nullable()
                .WithColumn("RejectReason").AsString().Nullable()
                .WithColumn("DateCreated").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow);
        }

        public override void Down()
        {
            Delete.Table("Documents");
            Delete.Schema("documents");
        }
    }
}