using Abp.Application.Features;
using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using Syntaq.Falcon.Authorization;

namespace Syntaq.Falcon.Web.Areas.Falcon.Startup
{
    public class FalconNavigationProvider : NavigationProvider
    {
        public const string MenuName = "App";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Main Menu"));

            menu
                .AddItem(new MenuItemDefinition(
                            FalconPageNames.Host.Dashboard,
                            L("Dashboard"),
                            url: "Falcon/HostDashboard",
                            icon: "fas fa-chart-line",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Dashboard)
                            )
                        )


                .AddItem(new MenuItemDefinition(
                        FalconPageNames.Tenant.Dashboard,
                        L("Dashboard"),
                        url: "Falcon/Dashboard",
                        icon: "fas fa-chart-line",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenant_Dashboard)
                    ))

                .AddItem(new MenuItemDefinition(
                    FalconPageNames.Host.Tenants,
                    L("Tenants"),
                    url: "Falcon/Tenants",
                    icon: "fas fa-building",
                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenants)
                    )
                )

                .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Host.Editions,
                                        L("Editions"),
                                        url: "Falcon/Editions",
                                        icon: "fas fa-shapes",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Editions)
                                    )
                    )

                                .AddItem(new MenuItemDefinition(
                                    FalconPageNames.Common.Projects,
                                    L("Projects"),
                                    icon: "fas fa-tasks",
                                    url: "Falcon/Projects",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Projects)
                                )
                                )

                                .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Common.Forms,
                                        L("Forms"),
                                        url: "Falcon/Forms",
                                        icon: "far fa-window-maximize",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Forms),
                                        featureDependency: new SimpleFeatureDependency(true, "App.Forms")
                                    )

                                )

                            .AddItem(new MenuItemDefinition(
                                    FalconPageNames.Tenant.Records,
                                    L("Records"),
                                    icon: "fas fa-folder",
                                    url: "Falcon/Records",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Records)
                            ))

                .AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.Templates,
                            L("TemplateTools"),
                            icon: "fas fa-tools",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Forms_Edit)
                            )
                                .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Common.Apps,
                                        L("Apps"),
                                        url: "Falcon/Apps",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Apps_Create),
                                        featureDependency: new SimpleFeatureDependency(true, "App.AppBuilder")
                                    )
                                )
                                .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Tenant.Templates,
                                        L("Documents"),
                                        url: "Falcon/Templates",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Templates_Create)
                                    )
                                )
                                .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Common.Forms,
                                        L("Forms"),
                                        url: "Falcon/Forms",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Forms),
                                        featureDependency: new SimpleFeatureDependency(true, "App.Forms")
                                    )

                                )

                                                            .AddItem(new MenuItemDefinition(
                                        FalconPageNames.Common.FormFeedbacks,
                                        L("FormFeedbacks"),
                                        url: "Falcon/FormFeedbacks",
                                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_FormFeedbacks)
                                )
                            )

                                        .AddItem(new MenuItemDefinition(
                                            FalconPageNames.Common.ProjectTemplates,
                                            L("Projects"),
                                            url: "Falcon/ProjectTemplates",
                                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Projects)
                                            )
                                        )


                                    //.AddItem(new MenuItemDefinition(
                                    //        FalconPageNames.Common.ProjectDeployments,
                                    //        L("ProjectDeployments"),
                                    //        url: "Falcon/ProjectDeployments",
                                    //        //icon: "fa fa-box-up",
                                    //        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_ProjectDeployments)
                                    //    )
                                    //)
                                    //.AddItem(new MenuItemDefinition(
                                    //        FalconPageNames.Common.ProjectTenants,
                                    //        L("ProjectTenants"),
                                    //        url: "Falcon/ProjectTenants",
                                    //        icon: "flaticon-more",
                                    //        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_ProjectTenants)
                                    //    )
                                    //)
                                    //.AddItem(new MenuItemDefinition(
                                    //        FalconPageNames.Common.ProjectReleases,
                                    //        L("ProjectReleases"),
                                    //        url: "Falcon/ProjectReleases",
                                    //        icon: "flaticon-more",
                                    //        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_ProjectReleases)
                                    //    )
                                    //)
                                    .AddItem(new MenuItemDefinition(
                                            FalconPageNames.Common.ProjectEnvironments,
                                            L("ProjectEnvironments"),
                                            url: "Falcon/ProjectEnvironments",
                                            //icon: "flaticon-more",
                                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_ProjectEnvironments)
                                        )
                                    )


                                    .AddItem(new MenuItemDefinition(
                                            FalconPageNames.Common.Tags,
                                            L("Tags"),
                                            url: "Falcon/Tags",
                                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tags)
                                        )
                                    )

                                     .AddItem(new MenuItemDefinition(
                                            FalconPageNames.Common.Vouchers,
                                            L("Vouchers"),
                                            url: "Falcon/Vouchers",
                                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Vouchers_Create)
                                        //requiredPermissionName: AppPermissions.Pages_Vouchers
                                        )

                                    )

                    )

                    .AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.Compliance,
                            L("Compliance"),
                            icon: "fas fa-user-shield",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration)
                            )

                            .AddItem(new MenuItemDefinition(
                                    FalconPageNames.Common.AuditLogs,
                                    L("AuditLogs"),
                                    url: "Falcon/AuditLogs",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_AuditLogs)
                                )
                            )

                            .AddItem(new MenuItemDefinition(
                                                    FalconPageNames.Common.Asic,
                                                    L("Asic"),
                                                    url: "Falcon/Asic",
                                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Asic),
                                                    featureDependency: new SimpleFeatureDependency(true, "App.Asic")
                                                )
                                            )

                            .AddItem(new MenuItemDefinition(
                                FalconPageNames.Common.EntityVersionHistories,
                                L("TemplateChangeLog"),
                                url: "Falcon/EntityVersionHistories",
                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_EntityVersionHistories)
                            ))

                           .AddItem(new MenuItemDefinition(
                                FalconPageNames.Common.RecordPolicies,
                                L("RecordPolicies"),
                                url: "Falcon/RecordPolicies",
                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_RecordPolicies)
                                )
                            ).AddItem(new MenuItemDefinition(
                                    FalconPageNames.Common.RecordPolicyActions,
                                    L("RecordPolicyActions"),
                                    url: "Falcon/RecordPolicyActions",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_RecordPolicyActions)
                                )
                            )

                            .AddItem(new MenuItemDefinition(
                                    FalconPageNames.Common.Submissions,
                                    L("Submissions"),
                                    url: "Falcon/Submissions",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Submissions_Admin)
                            ))

                            .AddItem(new MenuItemDefinition(
                                    FalconPageNames.Common.UserAcceptances,
                                    L("UserAcceptances"),
                                    url: "Falcon/UserAcceptances",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_UserAcceptances),
                                    featureDependency: new SimpleFeatureDependency(true, "App.UserAcceptance")
                                )
                            ).AddItem(new MenuItemDefinition(
                                    FalconPageNames.Host.UserAcceptanceTypes,
                                    L("UserAcceptanceTypes"),
                                    url: "Falcon/UserAcceptanceTypes",
                                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_UserAcceptanceTypes),
                                    featureDependency: new SimpleFeatureDependency(true, "App.UserAcceptance")
                                )
                            )
                            )

                 .AddItem(new MenuItemDefinition(
                        FalconPageNames.Common.Administration,
                        L("Administration"),
                        icon: "fas fa-cogs"
                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.OrganizationUnits,
                            L("Teams"),
                            url: "Falcon/OrganizationUnits",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_OrganizationUnits),
                            featureDependency: new SimpleFeatureDependency(true, "App.TeamsManagement")
                        )
                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.Roles,
                            L("Roles"),
                            url: "Falcon/Roles",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Roles),
                            featureDependency: new SimpleFeatureDependency(true, "App.RoleManagement")
                        )
                    )

                    .AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.Users,
                            L("Users"),
                            url: "Falcon/Users",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.Languages,
                            L("Languages"),
                            url: "Falcon/Languages",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Languages)
                        )
                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Host.Maintenance,
                            L("Maintenance"),
                            url: "Falcon/Maintenance",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Maintenance)
                        )

                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.UiCustomization,
                            L("VisualSettings"),
                            url: "Falcon/UiCustomization",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_UiCustomization),
                            featureDependency: new SimpleFeatureDependency(true, "App.VisualSettings")
                        )
                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Common.WebhookSubscriptions,
                            L("WebhookSubscriptions"),
                            url: "Falcon/WebhookSubscription",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_WebhookSubscription)
                        )

                    ).AddItem(new MenuItemDefinition(
                            FalconPageNames.Host.Settings,
                            L("Settings"),
                            url: "Falcon/HostSettings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Settings)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            FalconPageNames.Tenant.Settings,
                            L("Settings"),
                            url: "Falcon/Settings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Tenant_Settings)
                        )
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FalconConsts.LocalizationSourceName);
        }
    }
}