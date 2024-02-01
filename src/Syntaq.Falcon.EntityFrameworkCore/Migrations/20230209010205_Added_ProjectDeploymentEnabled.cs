using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_ProjectDeploymentEnabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "SfaProjectDeployments",
                type: "bit",
                nullable: false,
                defaultValue: false);
 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "SfaProjectDeployments");

        }
    }
}
