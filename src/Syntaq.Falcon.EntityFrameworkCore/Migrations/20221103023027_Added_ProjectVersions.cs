using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_ProjectVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SfaASICRequests_SfaRecordMatterItems_RecordMatterItemId",
                table: "SfaASICRequests");




            migrationBuilder.DropForeignKey(
                name: "FK_SfaProjects_SfaRecords_RecordId",
                table: "SfaProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterContributors_AbpUsers_UserId",
                table: "SfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterContributors_SfaForms_FormId",
                table: "SfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterContributors_SfaRecordMatters_RecordMatterId",
                table: "SfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaForms_FormId",
                table: "SfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaRecordMatterItems_RecordMatterItemId",
                table: "SfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaSubmissions_SubmissionId",
                table: "SfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaUserAcceptances_SfaRecordMatterContributors_RecordMatterContributorId",
                table: "SfaUserAcceptances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SfaRecordMatterItemHistories",
                table: "SfaRecordMatterItemHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SfaRecordMatterContributors",
                table: "SfaRecordMatterContributors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SfaProjects",
                table: "SfaProjects");

            migrationBuilder.DropIndex(
                name: "IX_SfaASICRequests_RecordMatterId",
                table: "SfaASICRequests");

            migrationBuilder.DropIndex(
                name: "IX_SfaASICRequests_RecordMatterItemId",
                table: "SfaASICRequests");

            migrationBuilder.RenameTable(
                name: "SfaRecordMatterItemHistories",
                newName: "sfaRecordMatterItemHistories");

            migrationBuilder.RenameTable(
                name: "SfaRecordMatterContributors",
                newName: "sfaRecordMatterContributors");

            migrationBuilder.RenameTable(
                name: "SfaProjects",
                newName: "sfaProjects");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterItemHistories_TenantId",
                table: "sfaRecordMatterItemHistories",
                newName: "IX_sfaRecordMatterItemHistories_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterItemHistories_SubmissionId",
                table: "sfaRecordMatterItemHistories",
                newName: "IX_sfaRecordMatterItemHistories_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterItemHistories_RecordMatterItemId",
                table: "sfaRecordMatterItemHistories",
                newName: "IX_sfaRecordMatterItemHistories_RecordMatterItemId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterItemHistories_FormId",
                table: "sfaRecordMatterItemHistories",
                newName: "IX_sfaRecordMatterItemHistories_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterContributors_UserId",
                table: "sfaRecordMatterContributors",
                newName: "IX_sfaRecordMatterContributors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterContributors_TenantId",
                table: "sfaRecordMatterContributors",
                newName: "IX_sfaRecordMatterContributors_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterContributors_RecordMatterId",
                table: "sfaRecordMatterContributors",
                newName: "IX_sfaRecordMatterContributors_RecordMatterId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaRecordMatterContributors_FormId",
                table: "sfaRecordMatterContributors",
                newName: "IX_sfaRecordMatterContributors_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_SfaProjects_TenantId",
                table: "sfaProjects",
                newName: "IX_sfaProjects_TenantId");





            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sfaProjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "sfaProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_sfaRecordMatterItemHistories",
                table: "sfaRecordMatterItemHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sfaRecordMatterContributors",
                table: "sfaRecordMatterContributors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sfaProjects",
                table: "sfaProjects",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SfaProjectEnvironments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    EnvironmentType = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaProjectEnvironments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaProjectReleases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Artifact = table.Column<byte>(type: "varbinary(max)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    VersionMajor = table.Column<int>(type: "int", nullable: false),
                    VersionMinor = table.Column<int>(type: "int", nullable: false),
                    VersionRevision = table.Column<int>(type: "int", nullable: false),
                    ReleaseType = table.Column<int>(type: "int", nullable: false),
                    ProjectEnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaProjectReleases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaProjectReleases_SfaProjectEnvironments_ProjectEnvironmentId",
                        column: x => x.ProjectEnvironmentId,
                        principalTable: "SfaProjectEnvironments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaProjectTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    SubscriberTenantId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ProjectEnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaProjectTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaProjectTenants_SfaProjectEnvironments_ProjectEnvironmentId",
                        column: x => x.ProjectEnvironmentId,
                        principalTable: "SfaProjectEnvironments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaProjectDeployments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    ProjectReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaProjectDeployments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaProjectDeployments_SfaProjectReleases_ProjectReleaseId",
                        column: x => x.ProjectReleaseId,
                        principalTable: "SfaProjectReleases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectDeployments_ProjectReleaseId",
                table: "SfaProjectDeployments",
                column: "ProjectReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectDeployments_TenantId",
                table: "SfaProjectDeployments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectEnvironments_TenantId",
                table: "SfaProjectEnvironments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectReleases_ProjectEnvironmentId",
                table: "SfaProjectReleases",
                column: "ProjectEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectReleases_TenantId",
                table: "SfaProjectReleases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectTenants_ProjectEnvironmentId",
                table: "SfaProjectTenants",
                column: "ProjectEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjectTenants_TenantId",
                table: "SfaProjectTenants",
                column: "TenantId");

 

            migrationBuilder.AddForeignKey(
                name: "FK_sfaProjects_SfaRecords_RecordId",
                table: "sfaProjects",
                column: "RecordId",
                principalTable: "SfaRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterContributors_AbpUsers_UserId",
                table: "sfaRecordMatterContributors",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterContributors_SfaForms_FormId",
                table: "sfaRecordMatterContributors",
                column: "FormId",
                principalTable: "SfaForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterContributors_SfaRecordMatters_RecordMatterId",
                table: "sfaRecordMatterContributors",
                column: "RecordMatterId",
                principalTable: "SfaRecordMatters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaForms_FormId",
                table: "sfaRecordMatterItemHistories",
                column: "FormId",
                principalTable: "SfaForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaRecordMatterItems_RecordMatterItemId",
                table: "sfaRecordMatterItemHistories",
                column: "RecordMatterItemId",
                principalTable: "SfaRecordMatterItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaSubmissions_SubmissionId",
                table: "sfaRecordMatterItemHistories",
                column: "SubmissionId",
                principalTable: "SfaSubmissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaUserAcceptances_sfaRecordMatterContributors_RecordMatterContributorId",
                table: "SfaUserAcceptances",
                column: "RecordMatterContributorId",
                principalTable: "sfaRecordMatterContributors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
 
            migrationBuilder.DropForeignKey(
                name: "FK_sfaProjects_SfaRecords_RecordId",
                table: "sfaProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterContributors_AbpUsers_UserId",
                table: "sfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterContributors_SfaForms_FormId",
                table: "sfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterContributors_SfaRecordMatters_RecordMatterId",
                table: "sfaRecordMatterContributors");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaForms_FormId",
                table: "sfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaRecordMatterItems_RecordMatterItemId",
                table: "sfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_sfaRecordMatterItemHistories_SfaSubmissions_SubmissionId",
                table: "sfaRecordMatterItemHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SfaUserAcceptances_sfaRecordMatterContributors_RecordMatterContributorId",
                table: "SfaUserAcceptances");

            migrationBuilder.DropTable(
                name: "SfaProjectDeployments");

            migrationBuilder.DropTable(
                name: "SfaProjectTenants");

            migrationBuilder.DropTable(
                name: "SfaProjectReleases");

            migrationBuilder.DropTable(
                name: "SfaProjectEnvironments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sfaRecordMatterItemHistories",
                table: "sfaRecordMatterItemHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sfaRecordMatterContributors",
                table: "sfaRecordMatterContributors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sfaProjects",
                table: "sfaProjects");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sfaProjects");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "sfaProjects");

            migrationBuilder.RenameTable(
                name: "sfaRecordMatterItemHistories",
                newName: "SfaRecordMatterItemHistories");

            migrationBuilder.RenameTable(
                name: "sfaRecordMatterContributors",
                newName: "SfaRecordMatterContributors");

            migrationBuilder.RenameTable(
                name: "sfaProjects",
                newName: "SfaProjects");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterItemHistories_TenantId",
                table: "SfaRecordMatterItemHistories",
                newName: "IX_SfaRecordMatterItemHistories_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterItemHistories_SubmissionId",
                table: "SfaRecordMatterItemHistories",
                newName: "IX_SfaRecordMatterItemHistories_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterItemHistories_RecordMatterItemId",
                table: "SfaRecordMatterItemHistories",
                newName: "IX_SfaRecordMatterItemHistories_RecordMatterItemId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterItemHistories_FormId",
                table: "SfaRecordMatterItemHistories",
                newName: "IX_SfaRecordMatterItemHistories_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterContributors_UserId",
                table: "SfaRecordMatterContributors",
                newName: "IX_SfaRecordMatterContributors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterContributors_TenantId",
                table: "SfaRecordMatterContributors",
                newName: "IX_SfaRecordMatterContributors_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterContributors_RecordMatterId",
                table: "SfaRecordMatterContributors",
                newName: "IX_SfaRecordMatterContributors_RecordMatterId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaRecordMatterContributors_FormId",
                table: "SfaRecordMatterContributors",
                newName: "IX_SfaRecordMatterContributors_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_sfaProjects_TenantId",
                table: "SfaProjects",
                newName: "IX_SfaProjects_TenantId");




            migrationBuilder.AddPrimaryKey(
                name: "PK_SfaRecordMatterItemHistories",
                table: "SfaRecordMatterItemHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SfaRecordMatterContributors",
                table: "SfaRecordMatterContributors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SfaProjects",
                table: "SfaProjects",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_RecordMatterId",
                table: "SfaASICRequests",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_RecordMatterItemId",
                table: "SfaASICRequests",
                column: "RecordMatterItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaASICRequests_SfaRecordMatterItems_RecordMatterItemId",
                table: "SfaASICRequests",
                column: "RecordMatterItemId",
                principalTable: "SfaRecordMatterItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

   


            migrationBuilder.AddForeignKey(
                name: "FK_SfaProjects_SfaRecords_RecordId",
                table: "SfaProjects",
                column: "RecordId",
                principalTable: "SfaRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterContributors_AbpUsers_UserId",
                table: "SfaRecordMatterContributors",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterContributors_SfaForms_FormId",
                table: "SfaRecordMatterContributors",
                column: "FormId",
                principalTable: "SfaForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterContributors_SfaRecordMatters_RecordMatterId",
                table: "SfaRecordMatterContributors",
                column: "RecordMatterId",
                principalTable: "SfaRecordMatters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaForms_FormId",
                table: "SfaRecordMatterItemHistories",
                column: "FormId",
                principalTable: "SfaForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaRecordMatterItems_RecordMatterItemId",
                table: "SfaRecordMatterItemHistories",
                column: "RecordMatterItemId",
                principalTable: "SfaRecordMatterItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaRecordMatterItemHistories_SfaSubmissions_SubmissionId",
                table: "SfaRecordMatterItemHistories",
                column: "SubmissionId",
                principalTable: "SfaSubmissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SfaUserAcceptances_SfaRecordMatterContributors_RecordMatterContributorId",
                table: "SfaUserAcceptances",
                column: "RecordMatterContributorId",
                principalTable: "SfaRecordMatterContributors",
                principalColumn: "Id");
        }
    }
}
