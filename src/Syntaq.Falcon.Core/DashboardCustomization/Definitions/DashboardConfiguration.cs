using System.Collections.Generic;
using System.Linq;
using Abp.Authorization;
using Abp.MultiTenancy;
using Syntaq.Falcon.Authorization;

namespace Syntaq.Falcon.DashboardCustomization.Definitions
{
    public class DashboardConfiguration
    {
        public List<DashboardDefinition> DashboardDefinitions { get; } = new List<DashboardDefinition>();

        public List<WidgetDefinition> WidgetDefinitions { get; } = new List<WidgetDefinition>();

        public List<WidgetFilterDefinition> WidgetFilterDefinitions { get; } = new List<WidgetFilterDefinition>();

        public DashboardConfiguration()
        {
            #region FilterDefinitions

            // These are global filter which all widgets can use
            var dateRangeFilter = new WidgetFilterDefinition(
                FalconDashboardCustomizationConsts.Filters.FilterDateRangePicker,
                "FilterDateRangePicker"
            );

            WidgetFilterDefinitions.Add(dateRangeFilter);

            // Add your filters here

            #endregion

            #region WidgetDefinitions

            // Define Widgets

            #region TenantWidgets

            var tenantWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.Pages_Tenant_Dashboard
            };
            var simplePermissionDependencyForTenantDashboard = new SimplePermissionDependency(AppPermissions.Pages_Tenant_Dashboard);

            var tenantWidgetsAdminPermission = new List<string>
            {
                AppPermissions.Pages_Tenant_Dashboard_Admin
            };

            var dailySales = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DailySales,
                "WidgetDailySales",
                side: MultiTenancySides.Tenant,
                usedWidgetFilters: new List<string> { dateRangeFilter.Id },
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var generalStats = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.GeneralStats,
                "WidgetGeneralStats",
                side: MultiTenancySides.Tenant,
                permissionDependency: new SimplePermissionDependency(
                    requiresAll: true,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Administration_AuditLogs
                )
            );

            var profitShare = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ProfitShare,
                "WidgetProfitShare",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var memberActivity = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.MemberActivity,
                "WidgetMemberActivity",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var regionalStats = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.RegionalStats,
                "WidgetRegionalStats",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var salesSummary = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.SalesSummary,
                "WidgetSalesSummary",
                usedWidgetFilters: new List<string>() { dateRangeFilter.Id },
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            //var topStats = new WidgetDefinition(
            //    FalconDashboardCustomizationConsts.Widgets.Tenant.TopStats,
            //    "WidgetTopStats",
            //    side: MultiTenancySides.Tenant,
            //    permissionDependency: simplePermissionDependencyForTenantDashboard
            //);

            var submissionLimit = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.SubmissionLimit,
                "WidgetSubmissionLimit",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var todaySubmissions = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.TodaySubmissions,
                "WidgetTodaySubmissions",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var newUsers = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.NewUsers,
                "WidgetNewUsers",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var mostRecentSubmissions = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.MostRecentSubmissions,
                "WidgetMostRecentSubmissions",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var projectStatus = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatus,
                "WidgetProjectStatus",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var projectCycle = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectCycle,
                "WidgetProjectCycle",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var projectStatusChart = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatusChart,
                "WidgetProjectStatusChart",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var actionRequiredChart = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ActionRequiredChart,
                "WidgetActionRequiredChart",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var waitingContributor = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingContributor,
                "WidgetShareWithMe",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var waitingOthers = new WidgetDefinition(
                 FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingOthers,
                 "WidgetWaitingOthers",
                 side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard
            );

            var projectTemplateUsageCount = new WidgetDefinition(
                 FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectTemplateUsageCount,
                 "WidgetProjectTemplateUsageCount",
                 side: MultiTenancySides.Tenant,
                 permissionDependency: simplePermissionDependencyForTenantDashboard);

            var projectRecentDocuments = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectRecentDocuments,
                "WidgetProjectRecentActivities",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var projectRecentTemplates = new WidgetDefinition(
               FalconDashboardCustomizationConsts.Widgets.Tenant.RecentTemplates,
               "WidgetProjectRecentTemplates",
               side: MultiTenancySides.Tenant,
               permissionDependency: simplePermissionDependencyForTenantDashboard);

            //Document count----
            var documentOnNew = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnNew,
                "WidgetDocumentOnNew",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var documentOnDraft = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnDraft,
                "WidgetDocumentOnDraft",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var documentOnPublish = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnPublish,
                "WidgetDocumentOnPublish",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var documentOnFinal = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnFinal,
                "WidgetDocumentOnFinal",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var documentOnAll = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnAll,
                "WidgetDocumentOnAll",
                side: MultiTenancySides.Tenant,
                permissionDependency: simplePermissionDependencyForTenantDashboard);

            var StartNewProject = new WidgetDefinition(
               FalconDashboardCustomizationConsts.Widgets.Tenant.StartNewProject,
               "WidgetStartNewProject",
               side: MultiTenancySides.Tenant,
               permissionDependency: simplePermissionDependencyForTenantDashboard);



            WidgetDefinitions.Add(generalStats);
            WidgetDefinitions.Add(dailySales);
            WidgetDefinitions.Add(profitShare);
            WidgetDefinitions.Add(memberActivity);
            WidgetDefinitions.Add(regionalStats);
           // WidgetDefinitions.Add(topStats);
            //
            WidgetDefinitions.Add(salesSummary);
            WidgetDefinitions.Add(submissionLimit);
            WidgetDefinitions.Add(todaySubmissions);
            WidgetDefinitions.Add(newUsers);
            WidgetDefinitions.Add(mostRecentSubmissions);
            WidgetDefinitions.Add(projectStatus);
            WidgetDefinitions.Add(projectCycle);

            WidgetDefinitions.Add(projectStatusChart);
            WidgetDefinitions.Add(actionRequiredChart);
            WidgetDefinitions.Add(waitingContributor);
            WidgetDefinitions.Add(projectRecentDocuments);
            WidgetDefinitions.Add(documentOnNew);
            WidgetDefinitions.Add(documentOnDraft);
            WidgetDefinitions.Add(documentOnPublish);
            WidgetDefinitions.Add(documentOnFinal);
            WidgetDefinitions.Add(documentOnAll);
            WidgetDefinitions.Add(StartNewProject);
            WidgetDefinitions.Add(projectRecentTemplates);

            WidgetDefinitions.Add(waitingOthers);
            WidgetDefinitions.Add(projectTemplateUsageCount);

            // Add your tenant side widgets here

            #endregion

            #region HostWidgets

            var hostWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.Pages_Administration_Host_Dashboard
            };
            var simplePermissionDependencyForHostDashboard = new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Dashboard);

            var incomeStatistics = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Host.IncomeStatistics,
                "WidgetIncomeStatistics",
                side: MultiTenancySides.Host,
                permissionDependency: simplePermissionDependencyForHostDashboard
            );

            //var hostTopStats = new WidgetDefinition(
            //    FalconDashboardCustomizationConsts.Widgets.Host.TopStats,
            //    "WidgetTopStats",
            //    side: MultiTenancySides.Host,
            //    permissionDependency: simplePermissionDependencyForHostDashboard
            //);

            var editionStatistics = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Host.EditionStatistics,
                "WidgetEditionStatistics",
                side: MultiTenancySides.Host,
                permissionDependency: simplePermissionDependencyForHostDashboard
            );

            var subscriptionExpiringTenants = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Host.SubscriptionExpiringTenants,
                "WidgetSubscriptionExpiringTenants",
                side: MultiTenancySides.Host,
                permissionDependency: simplePermissionDependencyForHostDashboard
            );

            var recentTenants = new WidgetDefinition(
                FalconDashboardCustomizationConsts.Widgets.Host.RecentTenants,
                "WidgetRecentTenants",
                side: MultiTenancySides.Host,
                usedWidgetFilters: new List<string>() { dateRangeFilter.Id },
                permissionDependency: simplePermissionDependencyForHostDashboard
            );

            WidgetDefinitions.Add(incomeStatistics);
           // WidgetDefinitions.Add(hostTopStats);
            WidgetDefinitions.Add(editionStatistics);
            WidgetDefinitions.Add(subscriptionExpiringTenants);
            WidgetDefinitions.Add(recentTenants);

            // Add your host side widgets here

            #endregion

            #endregion

            #region DashboardDefinitions

            // Create dashboard
            var defaultTenantDashboard = new DashboardDefinition(
                FalconDashboardCustomizationConsts.DashboardNames.DefaultTenantDashboard,
                new List<string>
                {
                    //STQ MODIFIED
                    //generalStats.Id, 
                    //dailySales.Id, 
                    //profitShare.Id, 
                    //memberActivity.Id, 
                    //regionalStats.Id, 
                    //topStats.Id, 
                    //salesSummary.Id,
                    //
                    submissionLimit.Id,
                    todaySubmissions.Id,
                    newUsers.Id,
                    mostRecentSubmissions.Id,
                    projectStatus.Id,
                    projectCycle.Id,
                    projectStatusChart.Id,
                    actionRequiredChart.Id,
                    waitingContributor.Id,
                    projectRecentDocuments.Id,
                    documentOnNew.Id,
                    documentOnDraft.Id,
                    documentOnPublish.Id,
                    documentOnFinal.Id,
                    documentOnAll.Id,
                    StartNewProject.Id,
                    projectRecentTemplates.Id,
                    waitingOthers.Id,
                    projectTemplateUsageCount.Id
                });

            DashboardDefinitions.Add(defaultTenantDashboard);

            var defaultHostDashboard = new DashboardDefinition(
                FalconDashboardCustomizationConsts.DashboardNames.DefaultHostDashboard,
                new List<string>
                {
                    incomeStatistics.Id,
                   // hostTopStats.Id,
                    editionStatistics.Id,
                    subscriptionExpiringTenants.Id,
                    recentTenants.Id
                });

            DashboardDefinitions.Add(defaultHostDashboard);

            // Add your dashboard definition here

            #endregion

        }

    }
}
