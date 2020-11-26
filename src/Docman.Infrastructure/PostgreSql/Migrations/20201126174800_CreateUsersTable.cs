using System;
using FluentMigrator;

namespace Docman.Infrastructure.PostgreSql.Migrations
{
    [Migration(20201126174800)]
    public class CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Schema("users");

            Create.Table("Users")
                .InSchema("users")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Email").AsFixedLengthString(255).NotNullable()
                .WithColumn("FirebaseId").AsFixedLengthString(50).NotNullable()
                .WithColumn("DateCreated").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow);
        }

        public override void Down()
        {
            Delete.Table("Users").InSchema("users");
            Delete.Schema("users");
        }
    }
}