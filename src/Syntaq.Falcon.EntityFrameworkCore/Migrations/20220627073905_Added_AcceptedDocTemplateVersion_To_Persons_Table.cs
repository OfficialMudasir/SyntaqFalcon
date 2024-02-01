using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_AcceptedDocTemplateVersion_To_Persons_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcceptedDocTemplateVersion",
                table: "SfaUserAcceptances",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedDocTemplateVersion",
                table: "SfaUserAcceptances");
        }
    }
}
