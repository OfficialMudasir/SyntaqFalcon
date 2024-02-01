using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Syntaq.Falcon.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Syntaq Permissions

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var projectDeployments = pages.CreateChildPermission(AppPermissions.Pages_ProjectDeployments, L("ProjectDeployments"));
            projectDeployments.CreateChildPermission(AppPermissions.Pages_ProjectDeployments_Create, L("CreateNewProjectDeployment"));
            projectDeployments.CreateChildPermission(AppPermissions.Pages_ProjectDeployments_Edit, L("EditProjectDeployment"));
            projectDeployments.CreateChildPermission(AppPermissions.Pages_ProjectDeployments_Delete, L("DeleteProjectDeployment"));

            var projectTenants = pages.CreateChildPermission(AppPermissions.Pages_ProjectTenants, L("ProjectTenants"));
            projectTenants.CreateChildPermission(AppPermissions.Pages_ProjectTenants_Create, L("CreateNewProjectTenant"));
            projectTenants.CreateChildPermission(AppPermissions.Pages_ProjectTenants_Edit, L("EditProjectTenant"));
            projectTenants.CreateChildPermission(AppPermissions.Pages_ProjectTenants_Delete, L("DeleteProjectTenant"));

            var projectReleases = pages.CreateChildPermission(AppPermissions.Pages_ProjectReleases, L("ProjectReleases"));
            projectReleases.CreateChildPermission(AppPermissions.Pages_ProjectReleases_Create, L("CreateNewProjectRelease"));
            projectReleases.CreateChildPermission(AppPermissions.Pages_ProjectReleases_Edit, L("EditProjectRelease"));
            projectReleases.CreateChildPermission(AppPermissions.Pages_ProjectReleases_Delete, L("DeleteProjectRelease"));

            var projectEnvironments = pages.CreateChildPermission(AppPermissions.Pages_ProjectEnvironments, L("ProjectEnvironments"));
            projectEnvironments.CreateChildPermission(AppPermissions.Pages_ProjectEnvironments_Create, L("CreateNewProjectEnvironment"));
            projectEnvironments.CreateChildPermission(AppPermissions.Pages_ProjectEnvironments_Edit, L("EditProjectEnvironment"));
            projectEnvironments.CreateChildPermission(AppPermissions.Pages_ProjectEnvironments_Delete, L("DeleteProjectEnvironment"));

            var asic = pages.CreateChildPermission(AppPermissions.Pages_Asic, L("Asic"));
            asic.CreateChildPermission(AppPermissions.Pages_Asic_Create, L("CreateNewAsic"));
            asic.CreateChildPermission(AppPermissions.Pages_Asic_Edit, L("EditAsic"));

            var recordPolicyActions = pages.CreateChildPermission(AppPermissions.Pages_RecordPolicyActions, L("RecordPolicyActions"));
            recordPolicyActions.CreateChildPermission(AppPermissions.Pages_RecordPolicyActions_Create, L("CreateNewRecordPolicyAction"));
            recordPolicyActions.CreateChildPermission(AppPermissions.Pages_RecordPolicyActions_Edit, L("EditRecordPolicyAction"));
            recordPolicyActions.CreateChildPermission(AppPermissions.Pages_RecordPolicyActions_Delete, L("DeleteRecordPolicyAction"));

            var recordPolicies = pages.CreateChildPermission(AppPermissions.Pages_RecordPolicies, L("RecordPolicies"), multiTenancySides: MultiTenancySides.Host);
            recordPolicies.CreateChildPermission(AppPermissions.Pages_RecordPolicies_Create, L("CreateNewRecordPolicy"), multiTenancySides: MultiTenancySides.Host);
            recordPolicies.CreateChildPermission(AppPermissions.Pages_RecordPolicies_Edit, L("EditRecordPolicy"), multiTenancySides: MultiTenancySides.Host);
            recordPolicies.CreateChildPermission(AppPermissions.Pages_RecordPolicies_Delete, L("DeleteRecordPolicy"), multiTenancySides: MultiTenancySides.Host);

            var tagValues = pages.CreateChildPermission(AppPermissions.Pages_TagValues, L("TagValues"));
            tagValues.CreateChildPermission(AppPermissions.Pages_TagValues_Create, L("CreateNewTagValue"));
            tagValues.CreateChildPermission(AppPermissions.Pages_TagValues_Edit, L("EditTagValue"));
            tagValues.CreateChildPermission(AppPermissions.Pages_TagValues_Delete, L("DeleteTagValue"));

            var tagEntityTypes = pages.CreateChildPermission(AppPermissions.Pages_TagEntityTypes, L("TagEntityTypes"));
            tagEntityTypes.CreateChildPermission(AppPermissions.Pages_TagEntityTypes_Create, L("CreateNewTagEntityType"));
            tagEntityTypes.CreateChildPermission(AppPermissions.Pages_TagEntityTypes_Edit, L("EditTagEntityType"));
            tagEntityTypes.CreateChildPermission(AppPermissions.Pages_TagEntityTypes_Delete, L("DeleteTagEntityType"));

            var tagEntities = pages.CreateChildPermission(AppPermissions.Pages_TagEntities, L("TagEntities"));
            tagEntities.CreateChildPermission(AppPermissions.Pages_TagEntities_Create, L("CreateNewTagEntity"));
            tagEntities.CreateChildPermission(AppPermissions.Pages_TagEntities_Edit, L("EditTagEntity"));
            tagEntities.CreateChildPermission(AppPermissions.Pages_TagEntities_Delete, L("DeleteTagEntity"));

            var tags = pages.CreateChildPermission(AppPermissions.Pages_Tags, L("Tags"));
            tags.CreateChildPermission(AppPermissions.Pages_Tags_Create, L("CreateNewTag"));
            tags.CreateChildPermission(AppPermissions.Pages_Tags_Edit, L("EditTag"));
            tags.CreateChildPermission(AppPermissions.Pages_Tags_Delete, L("DeleteTag"));

            var entityVersionHistories = pages.CreateChildPermission(AppPermissions.Pages_EntityVersionHistories, L("EntityVersionHistories"));
            entityVersionHistories.CreateChildPermission(AppPermissions.Pages_EntityVersionHistories_Create, L("CreateNewEntityVersionHistory"));
            entityVersionHistories.CreateChildPermission(AppPermissions.Pages_EntityVersionHistories_Edit, L("EditEntityVersionHistory"));
            entityVersionHistories.CreateChildPermission(AppPermissions.Pages_EntityVersionHistories_Delete, L("DeleteEntityVersionHistory"));

            var recordMatterItemHistories = pages.CreateChildPermission(AppPermissions.Pages_RecordMatterItemHistories, L("RecordMatterItemHistories"));
            recordMatterItemHistories.CreateChildPermission(AppPermissions.Pages_RecordMatterItemHistories_Create, L("CreateNewRecordMatterItemHistory"));
            recordMatterItemHistories.CreateChildPermission(AppPermissions.Pages_RecordMatterItemHistories_Edit, L("EditRecordMatterItemHistory"));
            recordMatterItemHistories.CreateChildPermission(AppPermissions.Pages_RecordMatterItemHistories_Delete, L("DeleteRecordMatterItemHistory"));

            var recordMatterAudits = pages.CreateChildPermission(AppPermissions.Pages_RecordMatterAudits, L("RecordMatterAudits"));
            recordMatterAudits.CreateChildPermission(AppPermissions.Pages_RecordMatterAudits_Create, L("CreateNewRecordMatterAudit"));
            recordMatterAudits.CreateChildPermission(AppPermissions.Pages_RecordMatterAudits_Edit, L("EditRecordMatterAudit"));
            recordMatterAudits.CreateChildPermission(AppPermissions.Pages_RecordMatterAudits_Delete, L("DeleteRecordMatterAudit"));

            var userPasswordHistories = pages.CreateChildPermission(AppPermissions.Pages_UserPasswordHistories, L("UserPasswordHistories"));
            userPasswordHistories.CreateChildPermission(AppPermissions.Pages_UserPasswordHistories_Create, L("CreateNewUserPasswordHistory"));
            userPasswordHistories.CreateChildPermission(AppPermissions.Pages_UserPasswordHistories_Edit, L("EditUserPasswordHistory"));
            userPasswordHistories.CreateChildPermission(AppPermissions.Pages_UserPasswordHistories_Delete, L("DeleteUserPasswordHistory"));

            var formFeedbacks = pages.CreateChildPermission(AppPermissions.Pages_FormFeedbacks, L("FormFeedbacks"));
            formFeedbacks.CreateChildPermission(AppPermissions.Pages_FormFeedbacks_Create, L("CreateNewFormFeedback"));
            formFeedbacks.CreateChildPermission(AppPermissions.Pages_FormFeedbacks_Edit, L("EditFormFeedback"));
            formFeedbacks.CreateChildPermission(AppPermissions.Pages_FormFeedbacks_Delete, L("DeleteFormFeedback"));

            var recordMatterContributors = pages.CreateChildPermission(AppPermissions.Pages_RecordMatterContributors, L("RecordMatterContributors"));
            recordMatterContributors.CreateChildPermission(AppPermissions.Pages_RecordMatterContributors_Create, L("CreateNewRecordMatterContributor"));
            recordMatterContributors.CreateChildPermission(AppPermissions.Pages_RecordMatterContributors_Edit, L("EditRecordMatterContributor"));
            recordMatterContributors.CreateChildPermission(AppPermissions.Pages_RecordMatterContributors_Delete, L("DeleteRecordMatterContributor"));

            var projects = pages.CreateChildPermission(AppPermissions.Pages_Projects, L("Projects"));
            projects.CreateChildPermission(AppPermissions.Pages_Projects_Create, L("CreateNewProject"));
            projects.CreateChildPermission(AppPermissions.Pages_Projects_Edit, L("EditProject"));
            projects.CreateChildPermission(AppPermissions.Pages_Projects_Delete, L("DeleteProject"));
            projects.CreateChildPermission(AppPermissions.Pages_ProjectTemplates_Create, L("CreateProjectTemplates"));
            projects.CreateChildPermission(AppPermissions.Pages_ProjectTemplates_Edit, L("EditProjectTemplates"));
            projects.CreateChildPermission(AppPermissions.Pages_ProjectTemplates_Delete, L("DeleteProjectTemplates"));

            var voucherEntities = pages.CreateChildPermission(AppPermissions.Pages_VoucherEntities, L("VoucherEntities"));
            voucherEntities.CreateChildPermission(AppPermissions.Pages_VoucherEntities_Create, L("CreateNewVoucherEntity"));
            voucherEntities.CreateChildPermission(AppPermissions.Pages_VoucherEntities_Edit, L("EditVoucherEntity"));
            voucherEntities.CreateChildPermission(AppPermissions.Pages_VoucherEntities_Delete, L("DeleteVoucherEntity"));

            var voucherUsages = pages.CreateChildPermission(AppPermissions.Pages_VoucherUsages, L("VoucherUsages"));
            voucherUsages.CreateChildPermission(AppPermissions.Pages_VoucherUsages_Create, L("CreateNewVoucherUsage"));
            voucherUsages.CreateChildPermission(AppPermissions.Pages_VoucherUsages_Edit, L("EditVoucherUsage"));
            voucherUsages.CreateChildPermission(AppPermissions.Pages_VoucherUsages_Delete, L("DeleteVoucherUsage"));

            var vouchers = pages.CreateChildPermission(AppPermissions.Pages_Vouchers, L("Vouchers"));
            vouchers.CreateChildPermission(AppPermissions.Pages_Vouchers_Create, L("CreateNewVoucher"));
            vouchers.CreateChildPermission(AppPermissions.Pages_Vouchers_Edit, L("EditVoucher"));
            vouchers.CreateChildPermission(AppPermissions.Pages_Vouchers_Delete, L("DeleteVoucher"));

            var submissions = pages.CreateChildPermission(AppPermissions.Pages_Submissions, L("Submissions"));
            submissions.CreateChildPermission(AppPermissions.Pages_Submissions_Admin, L("AdminSubmission"));
            submissions.CreateChildPermission(AppPermissions.Pages_Submissions_Create, L("CreateNewSubmission"));
            submissions.CreateChildPermission(AppPermissions.Pages_Submissions_Edit, L("EditSubmission"));
            submissions.CreateChildPermission(AppPermissions.Pages_Submissions_Delete, L("DeleteSubmission"));

            var mergeTexts = pages.CreateChildPermission(AppPermissions.Pages_MergeTexts, L("MergeTexts"));
            mergeTexts.CreateChildPermission(AppPermissions.Pages_MergeTexts_Create, L("CreateNewMergeText"));
            mergeTexts.CreateChildPermission(AppPermissions.Pages_MergeTexts_Edit, L("EditMergeText"));
            mergeTexts.CreateChildPermission(AppPermissions.Pages_MergeTexts_Delete, L("DeleteMergeText"));

            var mergeTextItems = pages.CreateChildPermission(AppPermissions.Pages_MergeTextItems, L("MergeTextItems"));
            mergeTextItems.CreateChildPermission(AppPermissions.Pages_MergeTextItems_Create, L("CreateNewMergeTextItem"));
            mergeTextItems.CreateChildPermission(AppPermissions.Pages_MergeTextItems_Edit, L("EditMergeTextItem"));
            mergeTextItems.CreateChildPermission(AppPermissions.Pages_MergeTextItems_Delete, L("DeleteMergeTextItem"));

            var mergeTextItemValues = pages.CreateChildPermission(AppPermissions.Pages_MergeTextItemValues, L("MergeTextItemValues"));
            mergeTextItemValues.CreateChildPermission(AppPermissions.Pages_MergeTextItemValues_Create, L("CreateNewMergeTextItemValue"));
            mergeTextItemValues.CreateChildPermission(AppPermissions.Pages_MergeTextItemValues_Edit, L("EditMergeTextItemValue"));
            mergeTextItemValues.CreateChildPermission(AppPermissions.Pages_MergeTextItemValues_Delete, L("DeleteMergeTextItemValue"));

            //var reports = pages.CreateChildPermission(AppPermissions.Pages_Reports, L("Reports"));
            //reports.CreateChildPermission(AppPermissions.Pages_Reports_Create, L("CreateNewReport"));
            //reports.CreateChildPermission(AppPermissions.Pages_Reports_Edit, L("EditReport"));
            //reports.CreateChildPermission(AppPermissions.Pages_Reports_Delete, L("DeleteReport"));

            var formRules = pages.CreateChildPermission(AppPermissions.Pages_FormRules, L("FormRules"));
            formRules.CreateChildPermission(AppPermissions.Pages_FormRules_Create, L("CreateNewFormRule"));
            formRules.CreateChildPermission(AppPermissions.Pages_FormRules_Edit, L("EditFormRule"));
            formRules.CreateChildPermission(AppPermissions.Pages_FormRules_Delete, L("DeleteFormRule"));

            var files = pages.CreateChildPermission(AppPermissions.Pages_Files, L("Files"));
            files.CreateChildPermission(AppPermissions.Pages_Files_Create, L("CreateNewFile"));
            files.CreateChildPermission(AppPermissions.Pages_Files_Edit, L("EditFile"));
            files.CreateChildPermission(AppPermissions.Pages_Files_Delete, L("DeleteFile"));

            var appJobs = pages.CreateChildPermission(AppPermissions.Pages_AppJobs, L("AppJobs"));
            appJobs.CreateChildPermission(AppPermissions.Pages_AppJobs_Create, L("CreateNewAppJob"));
            appJobs.CreateChildPermission(AppPermissions.Pages_AppJobs_Edit, L("EditAppJob"));
            appJobs.CreateChildPermission(AppPermissions.Pages_AppJobs_Delete, L("DeleteAppJob"));

            var apps = pages.CreateChildPermission(AppPermissions.Pages_Apps, L("Apps"));
            apps.CreateChildPermission(AppPermissions.Pages_Apps_Create, L("CreateNewApp"));
            apps.CreateChildPermission(AppPermissions.Pages_Apps_Edit, L("EditApp"));
            apps.CreateChildPermission(AppPermissions.Pages_Apps_Delete, L("DeleteApp"));

            var forms = pages.CreateChildPermission(AppPermissions.Pages_Forms, L("Forms"));
            forms.CreateChildPermission(AppPermissions.Pages_Forms_Create, L("CreateNewForm"));
            forms.CreateChildPermission(AppPermissions.Pages_Forms_Edit, L("EditForm"));
            forms.CreateChildPermission(AppPermissions.Pages_Forms_Delete, L("DeleteForm"));

            var folders = pages.CreateChildPermission(AppPermissions.Pages_Folders, L("Folders"));
            folders.CreateChildPermission(AppPermissions.Pages_Folders_Create, L("CreateNewFolder"));
            folders.CreateChildPermission(AppPermissions.Pages_Folders_Edit, L("EditFolder"));
            folders.CreateChildPermission(AppPermissions.Pages_Folders_Delete, L("DeleteFolder"));

            var templates = pages.CreateChildPermission(AppPermissions.Pages_Templates, L("Templates"));
            templates.CreateChildPermission(AppPermissions.Pages_Templates_Create, L("CreateNewTemplate"));
            templates.CreateChildPermission(AppPermissions.Pages_Templates_Edit, L("EditTemplate"));
            templates.CreateChildPermission(AppPermissions.Pages_Templates_Delete, L("DeleteTemplate"));

            var acLs = pages.CreateChildPermission(AppPermissions.Pages_ACLs, L("ACLs"));
            acLs.CreateChildPermission(AppPermissions.Pages_ACLs_Create, L("CreateNewACL"));
            acLs.CreateChildPermission(AppPermissions.Pages_ACLs_Edit, L("EditACL"));
            acLs.CreateChildPermission(AppPermissions.Pages_ACLs_Delete, L("DeleteACL"));

            var documents = pages.CreateChildPermission(AppPermissions.Pages_Documents, L("Documents"));
            documents.CreateChildPermission(AppPermissions.Pages_Documents_Create, L("CreateNewDocument"));
            documents.CreateChildPermission(AppPermissions.Pages_Documents_Edit, L("EditDocument"));
            documents.CreateChildPermission(AppPermissions.Pages_Documents_Delete, L("DeleteDocument"));

            var recordMatterItems = pages.CreateChildPermission(AppPermissions.Pages_RecordMatterItems, L("RecordMatterItems"));
            recordMatterItems.CreateChildPermission(AppPermissions.Pages_RecordMatterItems_Create, L("CreateNewRecordMatterItem"));
            recordMatterItems.CreateChildPermission(AppPermissions.Pages_RecordMatterItems_Edit, L("EditRecordMatterItem"));
            recordMatterItems.CreateChildPermission(AppPermissions.Pages_RecordMatterItems_Delete, L("DeleteRecordMatterItem"));

            var recordMatters = pages.CreateChildPermission(AppPermissions.Pages_RecordMatters, L("RecordMatters"));
            recordMatters.CreateChildPermission(AppPermissions.Pages_RecordMatters_Create, L("CreateNewRecordMatter"));
            recordMatters.CreateChildPermission(AppPermissions.Pages_RecordMatters_Edit, L("EditRecordMatter"));
            recordMatters.CreateChildPermission(AppPermissions.Pages_RecordMatters_Delete, L("DeleteRecordMatter"));

            var records = pages.CreateChildPermission(AppPermissions.Pages_Records, L("Records"));
            records.CreateChildPermission(AppPermissions.Pages_Records_Create, L("CreateNewRecord"));
            records.CreateChildPermission(AppPermissions.Pages_Records_Edit, L("EditRecord"));
            records.CreateChildPermission(AppPermissions.Pages_Records_Delete, L("DeleteRecord"));


            var userProfile = pages.CreateChildPermission(AppPermissions.Pages_User_Profile, L("UserProfile"));
            userProfile.CreateChildPermission(AppPermissions.Pages_User_Profile_Create, L("CreateUserProfile"));
            userProfile.CreateChildPermission(AppPermissions.Pages_User_Profile_Edit, L("EditUserProfile"));
            userProfile.CreateChildPermission(AppPermissions.Pages_User_Profile_Delete, L("DeleteUserProfile"));

            // Standard APNETZERO Permissions
            // COMMON PERMISSIONS(FOR BOTH OF TENANTS AND HOST)

            var userAcceptances = pages.CreateChildPermission(AppPermissions.Pages_UserAcceptances, L("UserAcceptances"));

            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangeProfilePicture, L("UpdateUsersProfilePicture"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeDefaultLanguage, L("ChangeDefaultLanguage"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));

            var webhooks = administration.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription, L("Webhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Create, L("CreatingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Edit, L("EditingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_ChangeActivity, L("ChangingWebhookActivity"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Detail, L("DetailingSubscription"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ListSendAttempts, L("ListingSendAttempts"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ResendWebhook, L("ResendingWebhook"));

            var dynamicProperties = administration.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties, L("DynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Create, L("CreatingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Edit, L("EditingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Delete, L("DeletingDynamicProperties"));

            var dynamicPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue, L("DynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Create, L("CreatingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Edit, L("EditingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Delete, L("DeletingDynamicPropertyValue"));

            var dynamicEntityProperties = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties, L("DynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Create, L("CreatingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Edit, L("EditingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Delete, L("DeletingDynamicEntityProperties"));

            var dynamicEntityPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue, L("EntityDynamicPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Create, L("CreatingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Edit, L("EditingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Delete, L("DeletingDynamicEntityPropertyValue"));

            //TENANT-SPECIFIC PERMISSIONS

            var tenantDashboard = pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);
            tenantDashboard.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard_Admin, L("DashboardAdminWidgets"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            userAcceptances.CreateChildPermission(AppPermissions.Pages_UserAcceptances_Create, L("CreateNewUserAcceptance"), multiTenancySides: MultiTenancySides.Host);
            userAcceptances.CreateChildPermission(AppPermissions.Pages_UserAcceptances_Edit, L("EditUserAcceptance"), multiTenancySides: MultiTenancySides.Host);
            userAcceptances.CreateChildPermission(AppPermissions.Pages_UserAcceptances_Delete, L("DeleteUserAcceptance"), multiTenancySides: MultiTenancySides.Host);

            var userAcceptanceTypes = pages.CreateChildPermission(AppPermissions.Pages_UserAcceptanceTypes, L("UserAcceptanceTypes"), multiTenancySides: MultiTenancySides.Host);
            userAcceptanceTypes.CreateChildPermission(AppPermissions.Pages_UserAcceptanceTypes_Create, L("CreateNewUserAcceptanceType"), multiTenancySides: MultiTenancySides.Host);
            userAcceptanceTypes.CreateChildPermission(AppPermissions.Pages_UserAcceptanceTypes_Edit, L("EditUserAcceptanceType"), multiTenancySides: MultiTenancySides.Host);
            userAcceptanceTypes.CreateChildPermission(AppPermissions.Pages_UserAcceptanceTypes_Delete, L("DeleteUserAcceptanceType"), multiTenancySides: MultiTenancySides.Host);

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FalconConsts.LocalizationSourceName);
        }
    }
}