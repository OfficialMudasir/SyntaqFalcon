using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_UserAcceptances_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "SfaUserAcceptances",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "SfaUserAcceptances",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SfaUserAcceptances",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "SfaUserAcceptances",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "SfaUserAcceptances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "SfaUserAcceptances");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "SfaUserAcceptances");
        }
    }
}
