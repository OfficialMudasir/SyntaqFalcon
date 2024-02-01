using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Modified_Project_Added_ReleaseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseId",
                table: "sfaProjects",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "sfaProjects");
        }
    }
}
