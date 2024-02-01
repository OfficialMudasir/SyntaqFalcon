using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class add_TenantFK_add_ifAccepted_column_to_userAcceptanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IfAccepted",
                table: "SfaUserAcceptances",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_SfaUserAcceptances_AbpTenants_TenantId",
                table: "SfaUserAcceptances",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SfaUserAcceptances_AbpTenants_TenantId",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "IfAccepted",
                table: "SfaUserAcceptances");
        }
    }
}
