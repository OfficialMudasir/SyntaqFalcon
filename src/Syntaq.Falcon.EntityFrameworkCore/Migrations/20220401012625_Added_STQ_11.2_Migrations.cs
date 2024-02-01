using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syntaq.Falcon.Migrations
{
    public partial class Added_STQ_112_Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ABN",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressCO",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressCountry",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressPostCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressState",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressSub",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressCountry",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine1",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine2",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressPostCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressState",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingName",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressWork",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Entity",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressCO",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressCountry",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressLine1",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressLine2",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressPostCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressState",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityAddressSub",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FLT",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPaymentConfigured",
                table: "AbpUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalABN",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "AbpUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LogoPictureId",
                table: "AbpUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayOnAccount",
                table: "AbpUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentAccessToken",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCurrency",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentPublishableToken",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRefreshToken",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberMobile",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberWork",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileBackgroundPictureId",
                table: "AbpUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteURL",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SfaACLs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    TargetTenantId = table.Column<long>(type: "bigint", nullable: true),
                    Accepted = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaACLs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaACLs_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaACLs_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaAppJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaAppJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaApps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RulesSchema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaApps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaEntityVersionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1223)", maxLength: 1223, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PreviousVersion = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaEntityVersionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaEntityVersionHistories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_SfaFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaFolders_SfaFolders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SfaFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaFormTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaFormTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaMergeTexts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaMergeTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AppliedTenantId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaRecordPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaUserPasswordHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaUserPasswordHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaUserPasswordHistories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoOfUses = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfaDocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    OriginalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Document = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CurrentVersion = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockToTenant = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaDocumentTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaDocumentTemplates_SfaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "SfaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    RecordName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_SfaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecords_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecords_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecords_SfaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "SfaFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    OriginalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RulesSchema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Script = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RulesScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CurrentVersion = table.Column<int>(type: "int", nullable: false),
                    PaymentEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentProcess = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentRefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentPublishableToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormTypeId = table.Column<int>(type: "int", nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockOnBuild = table.Column<bool>(type: "bit", nullable: false),
                    LockToTenant = table.Column<bool>(type: "bit", nullable: false),
                    RequireAuth = table.Column<bool>(type: "bit", nullable: false),
                    CustomCssId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_SfaForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaForms_SfaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "SfaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SfaForms_SfaFormTypes_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "SfaFormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaMergeTextItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MergeTextId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaMergeTextItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaMergeTextItems_SfaMergeTexts_MergeTextId",
                        column: x => x.MergeTextId,
                        principalTable: "SfaMergeTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordPolicyActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AppliedTenantId = table.Column<int>(type: "int", nullable: false),
                    ExpireDays = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RecordStatus = table.Column<int>(type: "int", nullable: false),
                    RecordPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordPolicyActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordPolicyActions_SfaRecordPolicies_RecordPolicyId",
                        column: x => x.RecordPolicyId,
                        principalTable: "SfaRecordPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaTagEntityTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaTagEntityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaTagEntityTypes_SfaTags_TagId",
                        column: x => x.TagId,
                        principalTable: "SfaTags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaTagValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaTagValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaTagValues_SfaTags_TagId",
                        column: x => x.TagId,
                        principalTable: "SfaTags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaVoucherEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EntityKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaVoucherEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaVoucherEntities_SfaVouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "SfaVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaVoucherUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    DateRedeemed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaVoucherUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaVoucherUsages_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SfaVoucherUsages_SfaVouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "SfaVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    ProjectTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_SfaProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaProjects_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaProjects_SfaRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "SfaRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RecordMatterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    HasFiles = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    RulesSchema = table.Column<string>(type: "nvarchar(max)", maxLength: 65536, nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    RequireReview = table.Column<bool>(type: "bit", nullable: false),
                    RequireApproval = table.Column<bool>(type: "bit", nullable: false),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_SfaRecordMatters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatters_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatters_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatters_SfaRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "SfaRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaFormFeedBacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FeedbackFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedbackFormData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaFormFeedBacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaFormFeedBacks_SfaForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SfaForms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaFormRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Rule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaFormRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaFormRules_SfaForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SfaForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaMergeTextItemValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    MergeTextItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaMergeTextItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaMergeTextItemValues_SfaMergeTextItems_MergeTextItemId",
                        column: x => x.MergeTextItemId,
                        principalTable: "SfaMergeTextItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaTagEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    TagValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaTagEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaTagEntities_SfaTagValues_TagValueId",
                        column: x => x.TagValueId,
                        principalTable: "SfaTagValues",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordMatterAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 32000, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    RecordMatterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaRecordMatterAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterAudits_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterAudits_SfaRecordMatters_RecordMatterId",
                        column: x => x.RecordMatterId,
                        principalTable: "SfaRecordMatters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordMatterContributors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StepStatus = table.Column<int>(type: "int", nullable: false),
                    StepRole = table.Column<int>(type: "int", nullable: false),
                    StepAction = table.Column<int>(type: "int", nullable: false),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FormSchema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormPages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordMatterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailFrom = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EmailCC = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EmailBCC = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
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
                    table.PrimaryKey("PK_SfaRecordMatterContributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterContributors_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterContributors_SfaForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SfaForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterContributors_SfaRecordMatters_RecordMatterId",
                        column: x => x.RecordMatterId,
                        principalTable: "SfaRecordMatters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiresPayment = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VoucherAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChargeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecordMatterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_SfaSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_SfaAppJobs_AppJobId",
                        column: x => x.AppJobId,
                        principalTable: "SfaAppJobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_SfaApps_AppId",
                        column: x => x.AppId,
                        principalTable: "SfaApps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_SfaForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SfaForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_SfaRecordMatters_RecordMatterId",
                        column: x => x.RecordMatterId,
                        principalTable: "SfaRecordMatters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaSubmissions_SfaRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "SfaRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordMatterItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    HasDocument = table.Column<bool>(type: "bit", nullable: false),
                    LockOnBuild = table.Column<bool>(type: "bit", nullable: false),
                    Document = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordMatterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormURI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentTemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowedFormats = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowWordAssignees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowPdfAssignees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowHtmlAssignees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SfaRecordMatterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItems_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItems_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItems_SfaRecordMatters_RecordMatterId",
                        column: x => x.RecordMatterId,
                        principalTable: "SfaRecordMatters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItems_SfaSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "SfaSubmissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SfaASICRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    HTTPRequests = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestMethod = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordMatterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordMatterItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManualReview = table.Column<bool>(type: "bit", nullable: false),
                    Triggered = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaASICRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaASICRequests_SfaRecordMatterItems_RecordMatterItemId",
                        column: x => x.RecordMatterItemId,
                        principalTable: "SfaRecordMatterItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfaRecordMatterItemHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Document = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContributorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowedFormats = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RecordMatterItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfaRecordMatterItemHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItemHistories_SfaForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SfaForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItemHistories_SfaRecordMatterItems_RecordMatterItemId",
                        column: x => x.RecordMatterItemId,
                        principalTable: "SfaRecordMatterItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SfaRecordMatterItemHistories_SfaSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "SfaSubmissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordPolicyActions_RecordPolicyId",
                table: "RecordPolicyActions",
                column: "RecordPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordPolicyActions_TenantId",
                table: "RecordPolicyActions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaACLs_OrganizationUnitId",
                table: "SfaACLs",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaACLs_TenantId",
                table: "SfaACLs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaACLs_UserId",
                table: "SfaACLs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaAppJobs_TenantId",
                table: "SfaAppJobs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaApps_TenantId",
                table: "SfaApps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_RecordId",
                table: "SfaASICRequests",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_RecordMatterId",
                table: "SfaASICRequests",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_RecordMatterItemId",
                table: "SfaASICRequests",
                column: "RecordMatterItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaASICRequests_TenantId",
                table: "SfaASICRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaDocumentTemplates_FolderId",
                table: "SfaDocumentTemplates",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaDocumentTemplates_TenantId",
                table: "SfaDocumentTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaEntityVersionHistories_TenantId",
                table: "SfaEntityVersionHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaEntityVersionHistories_UserId",
                table: "SfaEntityVersionHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFolders_ParentId",
                table: "SfaFolders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFolders_TenantId",
                table: "SfaFolders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFormFeedBacks_FormId",
                table: "SfaFormFeedBacks",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFormFeedBacks_TenantId",
                table: "SfaFormFeedBacks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFormRules_FormId",
                table: "SfaFormRules",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaFormRules_TenantId",
                table: "SfaFormRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaForms_FolderId",
                table: "SfaForms",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaForms_FormTypeId",
                table: "SfaForms",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaForms_TenantId",
                table: "SfaForms",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaMergeTextItems_MergeTextId",
                table: "SfaMergeTextItems",
                column: "MergeTextId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaMergeTextItems_TenantId",
                table: "SfaMergeTextItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaMergeTextItemValues_MergeTextItemId",
                table: "SfaMergeTextItemValues",
                column: "MergeTextItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaMergeTextItemValues_TenantId",
                table: "SfaMergeTextItemValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaMergeTexts_TenantId",
                table: "SfaMergeTexts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjects_CreatorUserId",
                table: "SfaProjects",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjects_RecordId",
                table: "SfaProjects",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaProjects_TenantId",
                table: "SfaProjects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterAudits_RecordMatterId",
                table: "SfaRecordMatterAudits",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterAudits_TenantId",
                table: "SfaRecordMatterAudits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterAudits_UserId",
                table: "SfaRecordMatterAudits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterContributors_FormId",
                table: "SfaRecordMatterContributors",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterContributors_RecordMatterId",
                table: "SfaRecordMatterContributors",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterContributors_TenantId",
                table: "SfaRecordMatterContributors",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterContributors_UserId",
                table: "SfaRecordMatterContributors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItemHistories_FormId",
                table: "SfaRecordMatterItemHistories",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItemHistories_RecordMatterItemId",
                table: "SfaRecordMatterItemHistories",
                column: "RecordMatterItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItemHistories_SubmissionId",
                table: "SfaRecordMatterItemHistories",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItemHistories_TenantId",
                table: "SfaRecordMatterItemHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItems_OrganizationUnitId",
                table: "SfaRecordMatterItems",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItems_RecordMatterId",
                table: "SfaRecordMatterItems",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItems_SubmissionId",
                table: "SfaRecordMatterItems",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItems_TenantId",
                table: "SfaRecordMatterItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatterItems_UserId",
                table: "SfaRecordMatterItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatters_OrganizationUnitId",
                table: "SfaRecordMatters",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatters_RecordId",
                table: "SfaRecordMatters",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatters_TenantId",
                table: "SfaRecordMatters",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordMatters_UserId",
                table: "SfaRecordMatters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecordPolicies_TenantId",
                table: "SfaRecordPolicies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecords_FolderId",
                table: "SfaRecords",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecords_OrganizationUnitId",
                table: "SfaRecords",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecords_TenantId",
                table: "SfaRecords",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaRecords_UserId",
                table: "SfaRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_AppId",
                table: "SfaSubmissions",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_AppJobId",
                table: "SfaSubmissions",
                column: "AppJobId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_FormId",
                table: "SfaSubmissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_RecordId",
                table: "SfaSubmissions",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_RecordMatterId",
                table: "SfaSubmissions",
                column: "RecordMatterId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_TenantId",
                table: "SfaSubmissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaSubmissions_UserId",
                table: "SfaSubmissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagEntities_TagValueId",
                table: "SfaTagEntities",
                column: "TagValueId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagEntities_TenantId",
                table: "SfaTagEntities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagEntityTypes_TagId",
                table: "SfaTagEntityTypes",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagEntityTypes_TenantId",
                table: "SfaTagEntityTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTags_TenantId",
                table: "SfaTags",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagValues_TagId",
                table: "SfaTagValues",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaTagValues_TenantId",
                table: "SfaTagValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserPasswordHistories_TenantId",
                table: "SfaUserPasswordHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaUserPasswordHistories_UserId",
                table: "SfaUserPasswordHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVoucherEntities_TenantId",
                table: "SfaVoucherEntities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVoucherEntities_VoucherId",
                table: "SfaVoucherEntities",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVouchers_TenantId",
                table: "SfaVouchers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVoucherUsages_TenantId",
                table: "SfaVoucherUsages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVoucherUsages_UserId",
                table: "SfaVoucherUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SfaVoucherUsages_VoucherId",
                table: "SfaVoucherUsages",
                column: "VoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordPolicyActions");

            migrationBuilder.DropTable(
                name: "SfaACLs");

            migrationBuilder.DropTable(
                name: "SfaASICRequests");

            migrationBuilder.DropTable(
                name: "SfaDocumentTemplates");

            migrationBuilder.DropTable(
                name: "SfaEntityVersionHistories");

            migrationBuilder.DropTable(
                name: "SfaFormFeedBacks");

            migrationBuilder.DropTable(
                name: "SfaFormRules");

            migrationBuilder.DropTable(
                name: "SfaMergeTextItemValues");

            migrationBuilder.DropTable(
                name: "SfaProjects");

            migrationBuilder.DropTable(
                name: "SfaRecordMatterAudits");

            migrationBuilder.DropTable(
                name: "SfaRecordMatterContributors");

            migrationBuilder.DropTable(
                name: "SfaRecordMatterItemHistories");

            migrationBuilder.DropTable(
                name: "SfaTagEntities");

            migrationBuilder.DropTable(
                name: "SfaTagEntityTypes");

            migrationBuilder.DropTable(
                name: "SfaUserPasswordHistories");

            migrationBuilder.DropTable(
                name: "SfaVoucherEntities");

            migrationBuilder.DropTable(
                name: "SfaVoucherUsages");

            migrationBuilder.DropTable(
                name: "SfaRecordPolicies");

            migrationBuilder.DropTable(
                name: "SfaMergeTextItems");

            migrationBuilder.DropTable(
                name: "SfaRecordMatterItems");

            migrationBuilder.DropTable(
                name: "SfaTagValues");

            migrationBuilder.DropTable(
                name: "SfaVouchers");

            migrationBuilder.DropTable(
                name: "SfaMergeTexts");

            migrationBuilder.DropTable(
                name: "SfaSubmissions");

            migrationBuilder.DropTable(
                name: "SfaTags");

            migrationBuilder.DropTable(
                name: "SfaAppJobs");

            migrationBuilder.DropTable(
                name: "SfaApps");

            migrationBuilder.DropTable(
                name: "SfaForms");

            migrationBuilder.DropTable(
                name: "SfaRecordMatters");

            migrationBuilder.DropTable(
                name: "SfaFormTypes");

            migrationBuilder.DropTable(
                name: "SfaRecords");

            migrationBuilder.DropTable(
                name: "SfaFolders");

            migrationBuilder.DropColumn(
                name: "ABN",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressCO",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressCountry",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressPostCode",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressState",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AddressSub",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingAddressCountry",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingAddressLine1",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingAddressLine2",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingAddressPostCode",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingAddressState",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BillingName",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EmailAddressWork",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Entity",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressCO",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressCountry",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressLine1",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressLine2",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressPostCode",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressState",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityAddressSub",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "FLT",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "HasPaymentConfigured",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "LegalABN",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "LogoPictureId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PayOnAccount",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PaymentAccessToken",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PaymentCurrency",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PaymentPublishableToken",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PaymentRefreshToken",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberMobile",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberWork",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ProfileBackgroundPictureId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "WebsiteURL",
                table: "AbpUsers");
        }
    }
}
