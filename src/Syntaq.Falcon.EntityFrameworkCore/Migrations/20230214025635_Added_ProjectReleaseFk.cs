using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_ProjectReleaseFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sfaProjects_ReleaseId",
                table: "sfaProjects",
                column: "ReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaProjects_SfaProjectReleases_ReleaseId",
                table: "sfaProjects",
                column: "ReleaseId",
                principalTable: "SfaProjectReleases",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sfaProjects_SfaProjectReleases_ReleaseId",
                table: "sfaProjects");

            migrationBuilder.DropIndex(
                name: "IX_sfaProjects_ReleaseId",
                table: "sfaProjects");
        }
    }
}
