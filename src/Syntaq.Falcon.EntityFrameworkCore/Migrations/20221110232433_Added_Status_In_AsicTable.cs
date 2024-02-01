using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_Status_In_AsicTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SfaASICRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SfaASICRequests");
        }
    }
}
