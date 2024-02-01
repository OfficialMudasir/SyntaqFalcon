using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_SfaUserAcceptanceTypes_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SfaUserAcceptanceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaUserAcceptanceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaUserAcceptanceTypes_SfaDocumentTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SfaDocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptanceTypes_TemplateId",
                table: "SfaUserAcceptanceTypes",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptanceTypes_TenantId",
                table: "SfaUserAcceptanceTypes",
                column: "TenantId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SfaUserAcceptanceTypes");
        }
    }
}
