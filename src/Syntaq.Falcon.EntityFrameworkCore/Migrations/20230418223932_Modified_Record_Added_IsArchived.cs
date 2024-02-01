using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Modified_Record_Added_IsArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "SfaRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "SfaRecords");
        }
    }
}
