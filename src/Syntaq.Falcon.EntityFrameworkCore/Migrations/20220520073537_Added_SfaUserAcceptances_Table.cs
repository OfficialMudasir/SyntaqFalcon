using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_SfaUserAcceptances_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SfaUserAcceptances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Accepted = table.Column<string>(maxLength: 128, nullable: true),
                    UserAcceptanceTypeId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    RecordMatterContributorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaUserAcceptances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaUserAcceptances_sfaRecordMatterContributors_RecordMatterContributorId",
                        column: x => x.RecordMatterContributorId,
                        principalTable: "sfaRecordMatterContributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SfaUserAcceptances_SfaUserAcceptanceTypes_UserAcceptanceTypeId",
                        column: x => x.UserAcceptanceTypeId,
                        principalTable: "SfaUserAcceptanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SfaUserAcceptances_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptances_RecordMatterContributorId",
                table: "SfaUserAcceptances",
                column: "RecordMatterContributorId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptances_TenantId",
                table: "SfaUserAcceptances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptances_UserAcceptanceTypeId",
                table: "SfaUserAcceptances",
                column: "UserAcceptanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserAcceptances_UserId",
                table: "SfaUserAcceptances",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SfaUserAcceptances");
        }
    }
}
