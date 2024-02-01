using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class remove_unnecessary_accepted_column_from_userAcceptanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "IfAccepted",
                table: "SfaUserAcceptances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accepted",
                table: "SfaUserAcceptances",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IfAccepted",
                table: "SfaUserAcceptances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
