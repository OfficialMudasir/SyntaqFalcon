using Abp.MultiTenancy;
using Abp.Zero.Configuration;

namespace Syntaq.Falcon.Authorization.Roles
{
    public static class AppRoleConfig
    {
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            //Static host roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.Admin,
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: true)
                );

            //Static tenant roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Admin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: true)
                );

            StaticRoleDefinition userrole = new StaticRoleDefinition(
                         StaticRoleNames.Tenants.User,
                         MultiTenancySides.Tenant);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Apps);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Create);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Edit);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Create);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Edit);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Folders);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Templates);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Create);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Edit);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Documents);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Records);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Forms);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Create);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Edit);
            //userrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Delete);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Delete);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Edit);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Delete);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Edit);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Tenant_Dashboard);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Tenant_Dashboard_Admin);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Delete);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Edit);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Files);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Delete);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Edit);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Projects);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Delete);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Edit);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_Tags);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntities);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntityTypes);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_TagValues);

            userrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Edit);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Create);
            userrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Delete);

            roleManagementConfig.StaticRoles.Add(userrole);


            // STQ MODIFIED START
            // AUTHOR ROLE START
            StaticRoleDefinition authorrole = new StaticRoleDefinition(
                          StaticRoleNames.Tenants.Author,
                          MultiTenancySides.Tenant);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Apps);
            //authorrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Create);
            //authorrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Edit);
            //authorrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Folders);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Templates);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Documents);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Records);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Forms);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Delete);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Delete);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Edit);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Delete);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Edit);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Tenant_Dashboard);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Delete);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Edit);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Files);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Delete);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Edit);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Projects);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Delete);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Edit);
            
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterAudits);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterAudits_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterAudits_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterAudits_Delete);

            //authorrole.GrantedPermissions.Add(AppPermissions.Pages_Administration_Users);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Administration_OrganizationUnits);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_Tags);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntities);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntityTypes);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_TagValues);

            authorrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Edit);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Create);
            authorrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Delete);

            roleManagementConfig.StaticRoles.Add(authorrole);

            //roleManagementConfig.StaticRoles.Add(
            //    new StaticRoleDefinition(
            //        StaticRoleNames.Tenants.User,
            //        MultiTenancySides.Tenant)
            //    );

            // AUTHOR ROLE END

            // BUILDER ROLE START

            StaticRoleDefinition builderrole = new StaticRoleDefinition(
                         StaticRoleNames.Tenants.Builder,
                         MultiTenancySides.Tenant);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Apps);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Apps_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_AppJobs_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Folders);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Folders_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Templates);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Templates_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ACLs_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Documents);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Documents_Delete);
 
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterItems_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatters_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Records);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Records_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Forms);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Forms_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormRules);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormRules_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormRules_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormRules_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_UserPasswordHistories_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_RecordMatterContributors_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Tenant_Dashboard);            

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_FormFeedbacks_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Files);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Files_Delete);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItems_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTexts_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_MergeTextItemValues_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Projects);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Projects_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ProjectTemplates_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ProjectTemplates_Delete);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_ProjectTemplates_Edit);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_Tags);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntities);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_TagEntityTypes);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_TagValues);

            builderrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Edit);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Create);
            builderrole.GrantedPermissions.Add(AppPermissions.Pages_User_Profile_Delete);

            roleManagementConfig.StaticRoles.Add(builderrole);

            // BUILDER ROLE END

        }
    }
}
